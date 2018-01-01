using System.Collections.Generic;
using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station.Population
{
    public class PeopleMover : QScript, IMessageListener, IPopMonitor
    {
        private readonly List<Person> _allPopulation = new List<Person>();
        private readonly List<PopContainerSet> _containers = new List<PopContainerSet>();

        internal void Initialize(PopulationControl populationControl)
        {
            
        }

        void Start()
        {
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopContainerSet.MessageName && args != null)
                HandleContainer(args as PopContainerSetMessageArgs);
        }

        public string Name { get { return "PeopleMover"; } }

        private void HandleContainer(PopContainerSetMessageArgs args)
        {
            if (args == null || args.PopContainerSet == null)
                throw new UnityException("PeopleMover given bad message data");

            _containers.Add(args.PopContainerSet);
            args.PopContainerSet.OnRemove += set => OnRemove(args.PopContainerSet);
        }

        private void OnRemove(PopContainerSet viewModel)
        {
            _containers.Remove(viewModel);
        }

        public void HandlePopulationUpdate(List<Person> people, bool wasAdded)
        {
            
        }

        public void HandleDeserialization(List<Person> people)
        {
            
        }
    }
}