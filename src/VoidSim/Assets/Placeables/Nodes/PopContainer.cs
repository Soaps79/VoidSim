using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Population;

namespace Assets.Placeables.Nodes
{
    [Serializable]
    public enum PopContainerType { Employment, Fulfillment, Transport }

    [Serializable]
    public class ContainerGenerationParams
    {
        public PopContainerType Type;
        public List<NeedsAffector> Affectors;
        public string ActivityPrefix;
    }

    public class PopContainerParams
    {
        public PopContainerType Type;
        public int MaxCapacity;
        public int Reserved;
        public string PlaceableName;
        public string ActivityPrefix;
        public List<NeedsAffector> Affectors = new List<NeedsAffector>();
    }

    public class PopContainerDetails
    {
        public string Name;
        public PopContainerType Type;
        public string PlaceableName;
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
        public string ActivityPrefix;
        public List<Person> CurrentOccupants = new List<Person>();
        public List<NeedsAffector> Affectors;
        public string PlaceableName;
        public bool HasRoom { get { return CurrentOccupants.Count < MaxCapacity; } }

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