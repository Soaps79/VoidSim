using System.Collections.Generic;
using Assets.Logistics;
using Assets.WorldMaterials.Population;

namespace Assets.Station.Population
{
    public interface IPeopleHandler
    {
        void HandlePopulationUpdate(List<Person> people, bool wasAdded);
        void HandleDeserialization(List<Person> people);
    }
}