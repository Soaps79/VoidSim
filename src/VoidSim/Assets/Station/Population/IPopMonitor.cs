using System.Collections.Generic;
using Assets.WorldMaterials.Population;

namespace Assets.Station.Population
{
    public interface IPopMonitor
    {
        void HandlePopulationUpdate(List<Person> people, bool wasAdded);
        void HandleDeserialization(List<Person> people);
    }
}