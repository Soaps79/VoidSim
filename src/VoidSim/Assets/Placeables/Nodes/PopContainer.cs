using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Population;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    [Serializable]
    public enum PopContainerType { Employment, Fulfillment, Transport }

    [Serializable]
    public class ContainerGenerationParams
    {
        public PopContainerType Type;
        public List<PersonNeedsValue> Affectors;
        public string ActivityPrefix;
    }

    public class PopContainerParams
    {
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public string PlaceableName;
        public string ActivityPrefix;
        public List<PersonNeedsValue> Affectors = new List<PersonNeedsValue>();
    }

    public class PopContainerDetails
    {
        public string Name;
        public PopContainerType Type;
        public string PlaceableName;
        public List<PersonNeedsValue> Affectors = new List<PersonNeedsValue>();
    }

    [Serializable]
    public class PopContainer
    {
        // these are left public for the editor, should never be set from outside
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public string Name;
        public string ActivityPrefix;
        public List<Occupancy> CurrentOccupancy = new List<Occupancy>();
        public List<PersonNeedsValue> Affectors;
        public string PlaceableName;
        [SerializeField] private bool _hasReserved;

        private int _actualOccupants;
        public bool HasRoom { get { return _actualOccupants < MaxCapacity; } }

        public Action OnUpdate;
        private void CheckUpdate()
        {
            if (OnUpdate != null)
                OnUpdate();
        }

        public PopContainer(PopContainerParams param)
        {
            Type = param.Type;
            MaxCapacity = param.MaxCapacity;
            Reserved = param.Reserved;
            PlaceableName = param.PlaceableName;
            Affectors = param.Affectors;
            ActivityPrefix = param.ActivityPrefix;

            EstablishOccupancy();
        }

        public void ApplyAffectors()
        {
            if (!Affectors.Any())
                return;

            for (int i = 0; i < CurrentOccupancy.Count; i++)
            {
                if (CurrentOccupancy[i].OccupiedBy != null)
                    CurrentOccupancy[i].OccupiedBy.ApplyAffectors(Affectors);
            }
        }

        private void EstablishOccupancy()
        {
            for (int i = 0; i < MaxCapacity; i++)
            {
                CurrentOccupancy.Add(new Occupancy());
            }
        }

        public void AddReserved(Person person)
        {
            var unoccupied = CurrentOccupancy.FirstOrDefault(i => !i.IsReserved);
            if (unoccupied == null) return;
            unoccupied.SetReserved(person);
            _hasReserved = true;
        }

        public void AddPerson(Person person)
        {
            if (person == null)
                return;

            // should add a check here to see if Person is already in Container
            
            Occupancy occupancy = null;
            if (_hasReserved)
                occupancy = CurrentOccupancy.FirstOrDefault(i => i.ReservedBy == person.Id);

            if (occupancy == null)
                occupancy = CurrentOccupancy.FirstOrDefault(i => !i.IsReserved && !i.IsOccupied);

            if (occupancy == null)
                return;

            occupancy.SetOccupant(person);
            _actualOccupants++;

            CheckUpdate();
        }

        public void RemovePerson(Person person)
        {
            var occupied = CurrentOccupancy.FirstOrDefault(i => i.OccupiedBy == person);
            if (occupied == null) return;

            occupied.SetOccupant(null);
            _actualOccupants--;

            CheckUpdate();
        }

        public void RemoveReserved(Person person)
        {
            var occupancy = CurrentOccupancy.FirstOrDefault(i => i.ReservedBy == person.Id);
            if (occupancy != null)
            {
                occupancy.SetReserved(null);
                CheckUpdate();
            }
        }
    }
}