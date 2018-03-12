using System;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Population;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    [Serializable]
    public class OccupancyData
    {
        public int Id;
        public int OccupiedById;
        public int ReservedById;
        public string ReservedByName;
    }

    [Serializable]
    public class Occupancy : ISerializeData<OccupancyData>
    {
        public Person OccupiedBy { get; private set; }
        public bool IsOccupied { get { return OccupiedBy != null; } }

        public int ReservedBy { get; private set; }
        public bool IsReserved { get { return ReservedBy != 0; } }

        // for editor debugging
        [SerializeField] private string _occupiedByName;
        [SerializeField] private string _reservedByName;

        public int Id { get; private set; }

        public Occupancy(int id)
        {
            Id = id;
        }

        private void CheckUpdate()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

        public void SetReserved(int id, string name)
        {
            ReservedBy = id;
            _reservedByName = name;
            CheckUpdate();
        }

        public void ClearReserved()
        {
            ReservedBy = 0;
            _reservedByName = string.Empty;
        }

        public void SetOccupant(Person person)
        {
            OccupiedBy = person;
            _occupiedByName = person.FullName;
            CheckUpdate();
        }

        public void ClearOccupant()
        {
            if (OccupiedBy == null) return;
            OccupiedBy = null;
            _occupiedByName = string.Empty;
            CheckUpdate();
        }

        public Action<Occupancy> OnUpdate;

        public void SetFromData(OccupancyData data)
        {
            Id = data.Id;
            ReservedBy = data.ReservedById;
            _reservedByName = data.ReservedByName;
            CheckUpdate();
        }

        public OccupancyData GetData()
        {
            return new OccupancyData
            {
                Id = Id,
                OccupiedById = OccupiedBy != null ? OccupiedBy.Id : 0,
                ReservedById = ReservedBy,
                ReservedByName = _reservedByName
            };
        }
    }
}