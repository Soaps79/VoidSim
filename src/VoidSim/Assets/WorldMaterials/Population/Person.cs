using System;
using Assets.Scripts.Serialization;

namespace Assets.WorldMaterials.Population
{
    [Serializable]
    public class PersonData
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public bool IsMale;
    }

    [Serializable]
    public class PersonNeeds
    {
        public string Name;
        public string DisplayName;
    }

    public class Person : ISerializeData<PersonData>
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public bool IsMale;

        public Person() { }
        
        public Person(PersonData data)
        {
            Id = data.Id;
            FirstName = data.FirstName;
            LastName = data.LastName;
            IsMale = data.IsMale;
        }

        public PersonData GetData()
        {
            return new PersonData
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                IsMale = IsMale
            };
        }
    }
}