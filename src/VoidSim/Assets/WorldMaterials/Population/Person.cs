using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
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

    [Serializable]
    public class PersonNeedsData
    {
        public PersonNeedsType Type;
        public float CurrentValue;
    }

    [Serializable]
    public enum PersonNeedsType
    {
        Rest, Entertainment
    }

    [Serializable]
    public class NeedsAffector
    {
        public PersonNeedsType Type;
        public float Value;
    }

    [Serializable]
    public class NeedsAffectorList
    {
        public List<NeedsAffector> Affectors;
    }

    [Serializable]
    public class PersonNeeds
    {
        public PersonNeedsType Type;
        public float CurrentValue;
        public float MaxValue;
        public float MinValue;
    }

    
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

        private readonly Dictionary<PersonNeedsType, PersonNeeds> 
            _needs = new Dictionary<PersonNeedsType, PersonNeeds>();

        public Person() { }

        public Person(PersonData data)
        {
            Id = data.Id;
            FirstName = data.FirstName;
            LastName = data.LastName;
            IsMale = data.IsMale;
            Home = data.Home;
            IsResident = data.IsResident;
            Employer = data.Employer;
        }

        public void SetNeeds(List<PersonNeeds> needs)
        {
            _needs.Clear();
            foreach (var need in needs)
            {
                _needs.Add(need.Type, need);
            }
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