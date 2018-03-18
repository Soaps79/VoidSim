using System;
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

    public interface IPersonWant
    {
        PopContainerType Type { get; }
        string GetDisplayName { get; }
        bool IsActive { get; set; }
    }

    /// <summary>
    /// Indicates that the Person is requesting fulfillment of their Needs
    /// </summary>
    public class FulfillmentWant : IPersonWant
    {
        public PopContainerType Type { get { return PopContainerType.Fulfillment; } }

        public string GetDisplayName
        {
            get
            {
                return UnfulfilledNeeds.Aggregate("",
                    (s, value) => s + string.Format("Wants fulfillment - {0}: {1}", value.Type, value.Amount));
            }
        }

        public bool IsActive { get; set; }

        public List<NeedsValue> UnfulfilledNeeds = new List<NeedsValue>();
    }

    /// <summary>
    /// Indicates that the Person would like to go to a bay to depart the station
    /// </summary>
    public class TransportWant : IPersonWant
    {
        public string ClientName;
        public PopContainerType Type { get { return PopContainerType.Transport; } }
        public string GetDisplayName { get { return "Requesting transport"; } }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Indicates that the Person's Needs are ulfilled and they are ready to work
    /// </summary>
    public class GoToWorkWant : IPersonWant
    {
        public PopContainerType Type { get { return PopContainerType.Employment; } }
        public string GetDisplayName { get { return "Ready to work"; } }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Handles signalling when a Person wants something enough to move for it.
    /// Note that all of the current needs are related to movement, this will have to be refactored when
    /// Wants are more robust than simply wanting to move.
    /// </summary>
    public class WantsHandler : ISerializeData<List<PersonNeedsData>>
    {
        private readonly Dictionary<PersonNeedsType, PersonNeedsValue>
            _needs = new Dictionary<PersonNeedsType, PersonNeedsValue>();

        private readonly List<NeedsValue> _lastAffected = new List<NeedsValue>();
        private readonly List<NeedsValue> _unfulfilledNeeds = new List<NeedsValue>();

        public bool IsAtWork { get; private set; }

        private readonly Dictionary<PopContainerType, IPersonWant> _wants = new Dictionary<PopContainerType, IPersonWant>();
        private static Dictionary<PersonNeedsType, PersonNeeds> _staticNeeds;

        public WantsHandler()
        {
            _wants.Add(PopContainerType.Employment, new GoToWorkWant());
            _wants.Add(PopContainerType.Fulfillment, new FulfillmentWant());
            _wants.Add(PopContainerType.Transport, new TransportWant());
        }

        public float OverallMood { get; private set; }

        public void SetNeeds(List<PersonNeedsValue> needs)
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
            OverallMood = _needs.Values.Average(i => i.Value);
        }

        // finds unfulfilled needs, puts them in order, and saves them locally
        private void RefreshUnfulfilledNeeds()
        {
            var list = new List<NeedsValue>();

            foreach (var need in _needs.Values)
            {
                if (need.Value < _staticNeeds[need.Type].MinTolerance)
                {
                    list.Add(new NeedsValue
                    {
                        Type = need.Type,
                        Amount = _staticNeeds[need.Type].MinTolerance - need.Value
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
        public void ApplyAffectors(List<PersonNeedsValue> affectors)
        {
            _lastAffected.Clear();
            foreach (var affector in affectors)
            {
                _needs[affector.Type].Value = Mathf.Clamp(
                    _needs[affector.Type].Value + affector.Value,
                    _staticNeeds[affector.Type].MinValue,
                    _staticNeeds[affector.Type].MaxValue);
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
                var chance = current.Value == 0 ? 1.0f : 1.0f - current.Value / _staticNeeds[need.Type].MinTolerance;
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
            _wants[PopContainerType.Fulfillment].IsActive = false;
            RefreshUnfulfilledNeeds();

            if (_unfulfilledNeeds.Any() && (isAlreadyTryingToFulfill || CheckWantsToFulfill(Random.value)))
            {
                var service = _wants[PopContainerType.Fulfillment] as FulfillmentWant;
                service.UnfulfilledNeeds.Clear();
                service.UnfulfilledNeeds.AddRange(_unfulfilledNeeds.ToList());
                service.IsActive = true;
                return;
            }

            // if no needs require attention, make sure the world knows person is ready to work
            if (!IsAtWork && CheckReadyToWork(Random.value))
            {
                _wants[PopContainerType.Employment].IsActive = true;
            }
        }

        public List<PersonNeedsValue> GetNeedsList()
        {
            return _needs.Values.ToList();
        }

        public float GetRest()
        {
            return _needs[PersonNeedsType.Rest].Value;
        }

        public float GetEntertainment()
        {
            return _needs[PersonNeedsType.Entertainment].Value;
        }

        public List<PersonNeedsData> GetData()
        {
            return _needs.Select(i => new PersonNeedsData
            {
                CurrentValue = i.Value.Value,
                Type = i.Value.Type
            }).ToList();
        }

        public bool CheckReadyToWork(float rand)
        {
            // no needs below 1.0, go to work
            if (!_needs.Values.All(i => i.Value >= 1.0))
                return true;

            PersonNeedsValue lowest = null;
            // find the need with the lowest value
            foreach (var need in _needs.Values)
            {
                if (need.Value < 1.0f)
                {
                    if (lowest == null)
                        lowest = need;
                    else if (need.Value < lowest.Value)
                        lowest = need;
                }
            }

            if (lowest == null)
                return true;

            // check its chance against the random number given
            var chanceRange = 1.0f - _staticNeeds[lowest.Type].StartWantingToMove;
            var range = 1.0f - _staticNeeds[lowest.Type].MinFulfillment;

            var chance = chanceRange + Mathf.Lerp(0, chanceRange, (lowest.Value - _staticNeeds[lowest.Type].MinFulfillment) / range);
            return chance > rand;
        }

        public static void SetStaticNeeds(Dictionary<PersonNeedsType, PersonNeeds> staticNeeds)
        {
            _staticNeeds = staticNeeds;
        }
    }
}