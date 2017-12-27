using System.Collections.Generic;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;

namespace Assets.Station.Population
{
    public class PeopleMover : QScript, IMessageListener, IPopMonitor
    {
        private readonly List<Person> _allPopulation = new List<Person>();

        internal void Initialize(PopulationControl populationControl)
        {
            
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            throw new System.NotImplementedException();
        }

        public string Name { get { return "PeopleMover"; } }

        public void HandlePopulationUpdate(List<Person> people, bool wasAdded)
        {
            
        }

        public void HandleDeserialization(List<Person> people)
        {
            
        }
    }
}