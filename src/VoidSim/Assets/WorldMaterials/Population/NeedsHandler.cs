using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
    public class NeedsValue
    {
        public PersonNeedsType Type;
        public float Amount;
    }

    public class NeedsHandler : ISerializeData<List<PersonNeedsData>>
    {
        private readonly Dictionary<PersonNeedsType, PersonNeeds>
            _needs = new Dictionary<PersonNeedsType, PersonNeeds>();

        private readonly List<NeedsValue> _lastAffected = new List<NeedsValue>();
        private readonly List<NeedsValue> _unfulfilledNeeds = new List<NeedsValue>();

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

        // finds unfulfilled needs, puts them in order, saves them locally and returns the list
        public List<NeedsValue> GetUnfulfilledNeeds()
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

            var toReturn = list.OrderByDescending(i => i.Amount).ToList();
            _unfulfilledNeeds.Clear();
            _unfulfilledNeeds.AddRange(toReturn);
            return toReturn;
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
        public bool CheckWantsToLeave(float rand)
        {
            foreach (var need in _unfulfilledNeeds)
            {
                // if this need is already being fulfilled, move on to the next
                var lastAffector = _lastAffected.FirstOrDefault(i => i.Type == need.Type && i.Amount > 0);
                if(lastAffector != null)
                    continue;

                // compare random to calculated chance to see if move is going to be requested
                var current = _needs[need.Type];
                var chance = Mathf.Lerp(0, 1, current.CurrentValue / current.MinTolerance);
                if (chance > rand)
                    return true;
            }
            return false;
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
            // now check its chance against the random number given
            var chanceRange = 1.0f - lowest.StartWantingToMove;
            var range = 1.0f - lowest.MinFulfillment;

            var chance = chanceRange + Mathf.Lerp(0, chanceRange, (lowest.CurrentValue - lowest.MinFulfillment) / range);
            return chance > rand;
        }
    }
}