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
        }

        public void ApplyAffectors()
        {
            if (!Affectors.Any())
                return;

            for (int i = 0; i < CurrentOccupancy.Count; i++)
            {
                if (CurrentOccupancy[i].Person != null)
                    CurrentOccupancy[i].Person.ApplyAffectors(Affectors);
            }
        }

        // The Set functions wrap basic functionality, and ensure that subscribers know
        // would normally do this using properties, but I want it to display in editor
        public void SetMaxCapacity(int capacity)
        {
            if (capacity == MaxCapacity)
                return;

            if(capacity < MaxCapacity)
                throw new UnityException("PopContainer MaxCapacity was lowered, not yet coded to handle this.");

            MaxCapacity = capacity;
            UpdateOccupancy();
            CheckUpdate();
        }

        private void UpdateOccupancy()
        {
            var previousCount = CurrentOccupancy.Count;
            for (int i = 0; i < MaxCapacity; i++)
            {
                if(i >= previousCount)
                    CurrentOccupancy.Add(new Occupancy());

                CurrentOccupancy[i].IsReserved = i < Reserved;
            }
        }

        // not sure if this will stay
        // currently exists to distinguish employee spots when they are away
        public void SetReserved(int reserved)
        {
            if (reserved == Reserved)
                return;

            Reserved = reserved;
            UpdateOccupancy();
            CheckUpdate();
        }

        public void AddPerson(Person person)
        {
            // should add some checks here to see if Person is already in Container

            for (var i = 0; i < CurrentOccupancy.Count; i++)
            {
                if (CurrentOccupancy[i].IsOccupied)
                    continue;

                CurrentOccupancy[i].Person = person;
                _actualOccupants++;
                break;
            }

            CheckUpdate();
        }

        public void RemovePerson(Person person)
        {
            for (var i = 0; i < CurrentOccupancy.Count; i++)
            {
                if (CurrentOccupancy[i].Person != person)
                    continue;

                CurrentOccupancy[i].Person = null;
                _actualOccupants--;
                break;
            }

            CheckUpdate();
        }
    }
}