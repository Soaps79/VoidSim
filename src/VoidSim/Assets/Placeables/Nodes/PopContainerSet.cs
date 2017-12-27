using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopHolderMessageArgs : MessageArgs
    {
        public PopContainerSet PopContainerSet;
    }

    /// <summary>
    /// Provides containers for a Person's physical presence
    /// Placeables like the Lounge, where you will have both workers and patrons, require 2 groupings
    /// </summary>
    public class PopContainerSet : PlaceableNode<PopContainerSet>
    {
        // for early debugging
        [SerializeField] private PopContainer _employmentContainer;
        [SerializeField] private PopContainer _serviceContainer;

        protected override PopContainerSet GetThis() { return this; }
        public const string MessageName = "PopContainerNodeCreated";
        public List<PopContainer> Containers = new List<PopContainer>();

        private readonly Dictionary<PopContainerType, PopContainer> _containers 
            = new Dictionary<PopContainerType, PopContainer>();

        public Action OnContainersUpdated;

        // creates a container, adds it to the list, returns it
        public PopContainer CreateContainer(PopContainerParams param)
        {
            var container = new PopContainer(param);
            container.OnUpdate += () =>
            {
                if (OnContainersUpdated != null) OnContainersUpdated();
            };
            if (param.Type == PopContainerType.Employment)
            {
                _employmentContainer = container;
            }
            else if (param.Type == PopContainerType.Service)
            {
                _serviceContainer = container;
            }
            _containers.Add(container.Type, container);
            return container;
        }

        public override void BroadcastPlacement()
        {
            Locator.MessageHub.QueueMessage(MessageName, new PopHolderMessageArgs{ PopContainerSet = this });
        }

        public override string NodeName { get { return "PopContainerSet"; } }
    }
}