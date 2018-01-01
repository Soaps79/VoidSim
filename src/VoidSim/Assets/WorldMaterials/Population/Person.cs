using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
    // Covers all progress needs for game serialization
    [Serializable]
    public class PersonData
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Home;
        public bool IsMale;
        public bool IsResident;
        public string Employer;
        public List<PersonNeedsData> Needs;
    }

    // Simplest form of needs for noting progress
    [Serializable]
    public class PersonNeedsData
    {
        public PersonNeedsType Type;
        public float CurrentValue;
    }

    // Should this be an enum, or list of strings?
    [Serializable]
    public enum PersonNeedsType
    {
        Rest, Entertainment
    }

    // used to apply a ticking change to needs while person is being affected
    [Serializable]
    public class NeedsAffector
    {
        public PersonNeedsType Type;
        public float Value;
    }

    // not really necessary, but makes it easier to drop needs into a PlaceableNode
    [Serializable]
    public class NeedsAffectorList
    {
        public List<NeedsAffector> Affectors;
    }

    // the version of needs that Person hangs onto during runtime
    [Serializable]
    public class PersonNeeds
    {
        public PersonNeedsType Type;
        public float CurrentValue;
        public float MaxValue;
        public float MinValue;
        public float MinTolerance;
    }

    // currently holding all data for a person and applying affectors
    [Serializable]
    public class Person : ISerializeData<PersonData>
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Home;
        public bool IsMale;
        public bool IsResident;
        public string Employer;
        public string CurrentlyOccupying;

        private readonly Dictionary<PersonNeedsType, PersonNeeds> 
            _needs = new Dictionary<PersonNeedsType, PersonNeeds>();

        [SerializeField] private List<PersonNeeds> _needsList = new List<PersonNeeds>();

        public Person() { }

        // used during deserialization
        public Person(PersonData data)
        {
            Id = data.Id;
            FirstName = data.FirstName;
            LastName = data.LastName;
            IsMale = data.IsMale;
            IsResident = data.IsResident;
            Home = data.Home;
            Employer = data.Employer;
        }

        public void SetNeeds(List<PersonNeeds> needs)
        {
            _needs.Clear();
            foreach (var need in needs)
            {
                _needs.Add(need.Type, need);
            }

            UpdateDebugOutput();
        }

        public void ApplyAffectors(List<NeedsAffector> affectors)
        {
            foreach (var affector in affectors)
            {
                _needs[affector.Type].CurrentValue = Mathf.Clamp(
                    _needs[affector.Type].CurrentValue + affector.Value, 
                    _needs[affector.Type].MinValue, 
                    _needs[affector.Type].MaxValue);
            }

            UpdateDebugOutput();
        }

        private void UpdateDebugOutput()
        {
            _needsList.Clear();
            _needsList.AddRange(_needs.Values.ToList());
        }

        public PersonData GetData()
        {
            return new PersonData
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                Home = Home,
                IsMale = IsMale,
                IsResident = IsResident,
                Employer = Employer,
                Needs = _needs.Select(i => new PersonNeedsData
                {
                    CurrentValue = i.Value.CurrentValue,
                    Type = i.Value.Type
                }).ToList()
            };
        }
    }
}