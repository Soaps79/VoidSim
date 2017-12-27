using System.Collections.Generic;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;

namespace Assets.Station.Population
{
    public class PeopleMover : QScript, IMessageListener
    {
        private readonly List<Person> _allPopulation = new List<Person>();

        public void AddPerson(Person person)
        {
            _allPopulation.Add(person);
        }

        internal void Initialize(PopulationControl populationControl)
        {
            //populationControl.OnPopulationUpdated += AddPerson;
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            throw new System.NotImplementedException();
        }

        public string Name { get { return "PeopleMover"; } }
    }
}