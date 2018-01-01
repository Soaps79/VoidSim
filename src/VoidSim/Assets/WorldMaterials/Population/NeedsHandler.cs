using System.Collections.Generic;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
    public class NeedsValue
    {
        public PersonNeedsType Type;
        public float Amount;
    }

    public static class NeedsHandler
    {
        public static List<NeedsValue> GetUnfulfilledNeeds(Dictionary<PersonNeedsType, PersonNeeds> needs)
        {
            var list = new List<NeedsValue>();

            foreach (var need in needs.Values)
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

            return list;
        }

        public static void ApplyAffectors(List<NeedsAffector> affectors, Dictionary<PersonNeedsType, PersonNeeds> needs)
        {
            foreach (var affector in affectors)
            {
                needs[affector.Type].CurrentValue = Mathf.Clamp(
                    needs[affector.Type].CurrentValue + affector.Value,
                    needs[affector.Type].MinValue,
                    needs[affector.Type].MaxValue);
            }
        }
    }
}