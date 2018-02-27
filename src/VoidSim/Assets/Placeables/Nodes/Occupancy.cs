using System;
using Assets.WorldMaterials.Population;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class Occupancy
    {
        public Person OccupiedBy { get; private set; }
        public bool IsOccupied { get { return OccupiedBy != null; } }

        public int ReservedBy { get; private set; }
        public bool IsReserved { get { return ReservedBy != 0; } }

        private void CheckUpdate()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

        // accepts null to "turn off" reserved
        public void SetReserved(Person person)
        {
            ReservedBy = person != null ? person.Id : 0;
            CheckUpdate();
        }

        // accepts null as "empty"
        public void SetOccupant(Person person)
        {
            OccupiedBy = person;
            CheckUpdate();
        }

        public Action<Occupancy> OnUpdate;
    }
}