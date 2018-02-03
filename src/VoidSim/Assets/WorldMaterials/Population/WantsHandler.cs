﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.WorldMaterials.Population
{
    public class NeedsValue
    {
        public PersonNeedsType Type;
        public float Amount;
    }

    public class WantsHandler : ISerializeData<List<PersonNeedsData>>
    {
        private readonly Dictionary<PersonNeedsType, PersonNeeds>
            _needs = new Dictionary<PersonNeedsType, PersonNeeds>();

        private readonly List<NeedsValue> _lastAffected = new List<NeedsValue>();
        private readonly List<NeedsValue> _unfulfilledNeeds = new List<NeedsValue>();

        public bool IsAtWork { get; private set; }

        private readonly Dictionary<PopContainerType, IPersonWant> _wants = new Dictionary<PopContainerType, IPersonWant>();

        public WantsHandler()
        {
            _wants.Add(PopContainerType.Employment, new GoToWorkWant());
            _wants.Add(PopContainerType.Fulfillment, new FulfillmentWant());
            _wants.Add(PopContainerType.Transport, new TransportWant());
        }

        public float OverallMood { get; private set; }

        public void SetNeeds(List<PersonNeeds> needs)
        {
            _needs.Clear();
            foreach (var need in needs)
            {
                _needs.Add(need.Type, need);
            }
            UpdateOverallMood();
        }

        private void UpdateOverallMood()
        {
            OverallMood = _needs.Values.Average(i => i.CurrentValue);
        }

        // finds unfulfilled needs, puts them in order, and saves them locally
        private void RefreshUnfulfilledNeeds()
        {
            var list = new List<NeedsValue>();

            foreach (var need in _needs.Values)
            {
                if (need.CurrentValue < need.MinTolerance)
                {
                    list.Add(new NeedsValue
                    {
                        Type = need.Type,
                        Amount = need.MinTolerance - need.CurrentValue
                    });
                }
            }

            var newNeeds = list.OrderByDescending(i => i.Amount).ToList();
            _unfulfilledNeeds.Clear();
            _unfulfilledNeeds.AddRange(newNeeds);
        }

        public void HandleLocationChange(PopContainerDetails details)
        {
            if (details.Type == PopContainerType.Employment)
            {
                _wants[PopContainerType.Employment].IsActive = false;
                IsAtWork = true;
            }
            else
            {
                IsAtWork = false;
            }
        }

        // applies the value and tracks it as the "last applied"
        public void ApplyAffectors(List<NeedsAffector> affectors)
        {
            _lastAffected.Clear();
            foreach (var affector in affectors)
            {
                _needs[affector.Type].CurrentValue = Mathf.Clamp(
                    _needs[affector.Type].CurrentValue + affector.Value,
                    _needs[affector.Type].MinValue,
                    _needs[affector.Type].MaxValue);
                _lastAffected.Add(new NeedsValue { Type = affector.Type, Amount = affector.Value });
            }
            UpdateOverallMood();
        }

        // iterates through unfulfilled needs and checks if any trigger him to want to move
        public bool CheckWantsToFulfill(float rand)
        {
            // this should be given another pass, when moving from one need to another needs to be more robust
            foreach (var need in _unfulfilledNeeds)
            {
                // if this need is already being fulfilled, move on to the next
                var lastAffector = _lastAffected.FirstOrDefault(i => i.Type == need.Type && i.Amount > 0);
                if(lastAffector != null)
                    continue;

                // compare random to calculated chance to see if move is going to be requested
                var current = _needs[need.Type];
                var chance = current.CurrentValue == 0 ? 1.0f : 1.0f - current.CurrentValue / current.MinTolerance;
                if (chance >= rand)
                    return true;
            }
            return false;
        }

        public bool IsRequesting(PopContainerType type)
        {
            return _wants[type].IsActive;
        }

        public IPersonWant GetRequested(PopContainerType type)
        {
            return _wants[type];
        }

        public void AssessNeeds()
        {
            // employment and transport are dominant needs until they are fulfilled
            if (_wants[PopContainerType.Employment].IsActive || _wants[PopContainerType.Transport].IsActive)
                return;

            // if the Person is already trying to fulfill, don't bother with the random check again
            var isAlreadyTryingToFulfill = _wants[PopContainerType.Fulfillment].IsActive;
            
            RefreshUnfulfilledNeeds();

            if (_unfulfilledNeeds.Any() && (isAlreadyTryingToFulfill || CheckWantsToFulfill(Random.value)))
            {
                var service = _wants[PopContainerType.Fulfillment] as FulfillmentWant;
                service.UnfulfilledNeeds.Clear();
                service.UnfulfilledNeeds.AddRange(_unfulfilledNeeds.ToList());
                return;
            }

            // if no needs require attention, make sure the world knows person is ready to work
            if (!IsAtWork && CheckReadyToWork(Random.value))
            {
                _wants[PopContainerType.Employment].IsActive = true;
            }
        }

        public List<PersonNeeds> GetNeedsList()
        {
            return _needs.Values.ToList();
        }

        public List<PersonNeedsData> GetData()
        {
            return _needs.Select(i => new PersonNeedsData
            {
                CurrentValue = i.Value.CurrentValue,
                Type = i.Value.Type
            }).ToList();
        }

        public bool CheckReadyToWork(float rand)
        {
            // no needs below 1.0, go to work
            if (!_needs.Values.All(i => i.CurrentValue >= 1.0))
                return true;

            PersonNeeds lowest = null;
            // find the need with the lowest value
            foreach (var need in _needs.Values)
            {
                if (need.CurrentValue < 1.0f)
                {
                    if (lowest == null)
                        lowest = need;
                    else if (need.CurrentValue < lowest.CurrentValue)
                        lowest = need;
                }
            }

            if (lowest == null)
                return true;

            // check its chance against the random number given
            var chanceRange = 1.0f - lowest.StartWantingToMove;
            var range = 1.0f - lowest.MinFulfillment;

            var chance = chanceRange + Mathf.Lerp(0, chanceRange, (lowest.CurrentValue - lowest.MinFulfillment) / range);
            return chance > rand;
        }
    }
}