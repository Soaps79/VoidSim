using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Population;

namespace Assets.Placeables.Nodes
{
    [Serializable]
    public enum PopContainerType { Employment, Service }

    public class PopContainerParams
    {
        public string Name;
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public List<NeedsAffector> Affectors = new List<NeedsAffector>();
    }

    [Serializable]
    public class PopContainer
    {
        // these are left public for the editor, should never be set from outside
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public string Name;
        public List<Person> CurrentOccupants = new List<Person>();
        public List<NeedsAffector> Affectors;

        public Action OnUpdate;
        private void CheckUpdate()
        {
            if (OnUpdate != null)
                OnUpdate();
        }

        public PopContainer(PopContainerParams param)
        {
            Name = param.Name;
            Type = param.Type;
            MaxCapacity = param.MaxCapacity;
            Reserved = param.Reserved;
            Affectors = param.Affectors;
        }

        public void ApplyAffectors()
        {
            if (!Affectors.Any())
                return;

            CurrentOccupants.ForEach(i => i.ApplyAffectors(Affectors));
        }

        // The Set functions wrap basic functionality, and ensure that subscribers know
        // would normally do this using properties, but I want it to display in editor
        public void SetMaxCapacity(int capacity)
        {
            if (capacity == MaxCapacity)
                return;

            MaxCapacity = capacity;
            CheckUpdate();
        }

        // not sure if this will stay
        // currently exists to distinguish employee spots when they are away
        public void SetReserved(int reserved)
        {
            if (reserved == Reserved)
                return;

            Reserved = reserved;
            CheckUpdate();
        }

        public void AddPerson(Person person, bool incrementReserve = false)
        {
            if (CurrentOccupants.Contains(person))
                return;

            CurrentOccupants.Add(person);
            if (incrementReserve)
                Reserved++;

            CheckUpdate();
        }

        public void RemovePerson(Person person, bool incrementReserve = false)
        {
            if (!CurrentOccupants.Remove(person))
                return;

            if(incrementReserve)
                Reserved++;

            CheckUpdate();
        }
    }
}