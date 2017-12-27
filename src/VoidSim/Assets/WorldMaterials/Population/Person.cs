using System;
using System.Collections.Generic;
using Assets.Scripts.Serialization;

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
    }

    [Serializable]
    public class PersonNeeds
    {
        public string Name;
        public string DisplayName;
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
                Employer = Employer
            };
        }
    }
}