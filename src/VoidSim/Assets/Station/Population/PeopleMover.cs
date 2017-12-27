using System.Collections.Generic;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;
using Sirenix.OdinInspector.Editor;

namespace Assets.Station.Population
{
    public class PeopleMover : QScript, IMessageListener
    {
        private readonly List<Person> _allPopulation = new List<Person>();
        
        public void Initialize()
        {
            
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            throw new System.NotImplementedException();
        }

        public string Name { get { return "PeopleMover"; } }

        public void AddPerson(Person person)
        {
            _allPopulation.Add(person);
        }
    }
}