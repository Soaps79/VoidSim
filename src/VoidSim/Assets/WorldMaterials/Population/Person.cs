using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public string CurrentlyOccupying;
        public int OccupancyId;
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

    // used to represent a need and its value
    [Serializable]
    public class PersonNeedsValue
    {
        public PersonNeedsType Type;
        public float Value;
    }

    // not really necessary, but makes it easier to drop needs into a PlaceableNode
    [Serializable]
    public class NeedsAffectorList
    {
        public List<PersonNeedsValue> Affectors;
    }

    // the version of needs that Person hangs onto during runtime
    [Serializable]
    public class PersonNeeds
    {
        public PersonNeedsType Type;
        public float MaxValue;
        public float MinValue;
        public float MinTolerance;
        public float MinFulfillment;
        public float StartWantingToMove;
    }

    // currently holding all data for a person and applying affectors
    [Serializable]
    public class Person : ISerializeData<PersonData>
    {
        public int Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                CheckUpdateCallback();
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == _firstName) return;
                _firstName = value;
                FullName = _firstName + " " + _lastName;
                CheckUpdateCallback();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value == _lastName) return;
                _lastName = value;
                FullName = _firstName + " " + _lastName;
                CheckUpdateCallback();
            }
        }

        public string FullName { get; private set; }

        public string Home
        {
            get { return _home; }
            set
            {
                if (value == _home) return;
                _home = value;
                CheckUpdateCallback();
            }
        }

        public bool IsMale
        {
            get { return _isMale; }
            set
            {
                if (value == _isMale) return;
                _isMale = value;
                CheckUpdateCallback();
            }
        }

        public bool IsResident
        {
            get { return _isResident; }
            set
            {
                if (value == _isResident) return;
                _isResident = value;
                CheckUpdateCallback();
            }
        }

        public string Employer
        {
            get { return _employer; }
            set
            {
                if (value == _employer) return;
                _employer = value;
                CheckUpdateCallback();
            }
        }

        public string CurrentlyOccupying
        {
            get { return _currentlyOccupying; }
            set
            {
                if (value == _currentlyOccupying) return;
                _currentlyOccupying = value;
                CheckUpdateCallback();
            }
        }

        public string CurrentActivity
        {
            get { return _currentActivity; }
            set
            {
                if (value == _currentActivity) return;
                _currentActivity = value;
                CheckUpdateCallback();
            }
        }

        // we do not want anything relying on this field updating, does not trigger callback like all other properties 
        // not pretty, but occupancy will be moved to its own model when its better fleshed out
        public int OccupancyId
        {
            get { return _occupancyId; }
            set
            {
                if (value == _occupancyId) return;
                _occupancyId = value;
            }
        }

        // serializing these so they're viewable in Unity editor
        [SerializeField] private int _id;
        [SerializeField] private string _firstName;
        [SerializeField] private string _lastName;
        [SerializeField] private string _home;
        [SerializeField] private bool _isMale;
        [SerializeField] private bool _isResident;
        [SerializeField] private string _employer;
        [SerializeField] private string _currentlyOccupying;
        [SerializeField] private string _currentActivity;
        [SerializeField] private int _occupancyId;
        
        public readonly WantsHandler Wants = new WantsHandler();
        
        [SerializeField] private List<PersonNeedsValue> _needsList = new List<PersonNeedsValue>();
        
        public Action<Person> OnUpdate;

        private void CheckUpdateCallback()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

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
            CurrentlyOccupying = data.CurrentlyOccupying;
            OccupancyId = data.OccupancyId;
        }

        public void SetNeeds(List<PersonNeedsValue> needs)
        {
            Wants.SetNeeds(needs);
            UpdateDebugOutput();
        }

        public void ApplyAffectors(List<PersonNeedsValue> affectors)
        {
            Wants.ApplyAffectors(affectors);
            UpdateDebugOutput();
        }

        public void AssessNeeds()
        {
            Wants.AssessNeeds();
        }

        public void HandleLocationChange(PopContainerDetails details)
        {
            Wants.HandleLocationChange(details);
        }

        private void UpdateDebugOutput()
        {
            _needsList.Clear();
            _needsList.AddRange(Wants.GetNeedsList());
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
                CurrentlyOccupying = CurrentlyOccupying,
                OccupancyId = OccupancyId,
                Needs = Wants.GetData()
            };
        }
    }
}