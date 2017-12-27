using System;
using System.Collections.Generic;
using Assets.WorldMaterials.Population;

namespace Assets.Placeables.Nodes
{
    [Serializable]
    public enum PopContainerType { Employment, Service }

    public class PopContainerParams
    {
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
    }

    [Serializable]
    public class PopContainer
    {
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public List<Person> CurrentOccupants = new List<Person>();

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
        public void SetReserved(int capacity)
        {
            if (capacity == MaxCapacity)
                return;

            MaxCapacity = capacity;
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