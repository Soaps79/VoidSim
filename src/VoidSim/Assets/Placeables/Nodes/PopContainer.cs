using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
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
    public class PopContainerData
    {
        public string Name;
        public List<OccupancyData> Occupancies;
    }

    [Serializable]
    public class PopContainer : ISerializeData<PopContainerData>
    {
        // these are left public for the editor, should never be set from outside
        public PopContainerType Type;
        public int MaxCapacity;
        public string Name;
        public string ActivityPrefix;
        public List<Occupancy> CurrentOccupancy = new List<Occupancy>();
        public List<PersonNeedsValue> Affectors;
        public string PlaceableName;

        [SerializeField] private bool _hasReserved;
        [SerializeField] private int _reservedCount;

        public int CurrentOccupantCount {  get { return _actualOccupants; } }
        [SerializeField] private int _actualOccupants;

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
            var currentId = 0;
            for (int i = 0; i < MaxCapacity; i++)
            {
                currentId++;
                CurrentOccupancy.Add(new Occupancy(currentId));
            }
        }

        public void AddReserved(Person person)
        {
            var unoccupied = CurrentOccupancy.FirstOrDefault(i => !i.IsReserved);
            if (unoccupied == null) return;
            unoccupied.SetReserved(person.Id, person.FullName);
            _reservedCount++;
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
            person.OccupancyId = occupancy.Id;
            _actualOccupants++;

            CheckUpdate();
        }

        public void ResumePerson(Person person)
        {
            var occupancy = CurrentOccupancy.FirstOrDefault(i => i.Id == person.OccupancyId);
            if (occupancy == null) return;
            _actualOccupants++;
            occupancy.SetOccupant(person);
            CheckUpdate();
        }

        public void RemovePerson(Person person)
        {
            var occupied = CurrentOccupancy.FirstOrDefault(i => i.OccupiedBy == person);
            if (occupied == null) return;

            occupied.ClearOccupant();
            _actualOccupants--;

            CheckUpdate();
        }

        public void RemoveReserved(Person person)
        {
            var occupancy = CurrentOccupancy.FirstOrDefault(i => i.ReservedBy == person.Id);
            if (occupancy != null)
            {
                occupancy.ClearReserved();
                _reservedCount--;
                CheckUpdate();
            }
        }

        public void SetFromData(PopContainerData data)
        {
            if(data.Occupancies.Count != CurrentOccupancy.Count)
                throw new UnityException("PopContainer occupants count does not match the one from data");

            _hasReserved = data.Occupancies.Any(i => i.ReservedById > 0);

            for (int i = 0; i < data.Occupancies.Count; i++)
            {
                CurrentOccupancy[i].SetFromData(data.Occupancies[i]);
            }
        }

        public PopContainerData GetData()
        {
            return new PopContainerData
            {
                Name = Name,
                Occupancies = CurrentOccupancy.Select(i => i.GetData()).ToList()
            };
        }
    }
}