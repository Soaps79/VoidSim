using System.Collections.Generic;
using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopHolder : QScript, IPeopleHolder
    {
        [SerializeField]
        private List<Person> _occupants;

        public void TakePeople(IEnumerable<Person> people)
        {
            _occupants = new List<Person>();
            _occupants.AddRange(people);
        }
    }
}