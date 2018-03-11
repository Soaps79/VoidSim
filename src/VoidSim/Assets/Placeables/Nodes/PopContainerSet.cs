using System;
using System.Collections.Generic;
using Assets.Scripts;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopContainerSetMessageArgs : MessageArgs
    {
        public PopContainerSet PopContainerSet;
    }

    /// <summary>
    /// Provides containers for a Person's physical presence
    /// Placeables like the Lounge, where you will have both workers and patrons, require 2 groupings
    /// </summary>
    public class PopContainerSet : PlaceableNode<PopContainerSet>
    {
        // for early debugging in editor
        [SerializeField] private PopContainer _employmentContainer;
        [SerializeField] private PopContainer _serviceContainer;

        [SerializeField] private TimeLength _updateTime;

        protected override PopContainerSet GetThis() { return this; }
        public const string MessageName = "PopContainerNodeCreated";

        public readonly List<PopContainer> Containers = new List<PopContainer>();

        public Action<List<PopContainer>> OnContainersUpdated;

        // creates a container, adds it to the list, returns it
        public PopContainer CreateContainer(PopContainerParams param)
        {
            var container = new PopContainer(param) {Name = (name + "_" + param.Type).ToLower()};

            if (param.Type == PopContainerType.Employment)
            {
                _employmentContainer = container;
            }
            else if (param.Type == PopContainerType.Fulfillment)
            {
                _serviceContainer = container;
            }
            Containers.Add(container);

            if (OnContainersUpdated != null) OnContainersUpdated(Containers);
            return container;
        }

        public override void Initialize(PlaceableData data)
        {
            var time = Locator.WorldClock.GetSeconds(_updateTime);
            var node = StopWatch.AddNode("apply_affectors", time);
            node.OnTick += UpdatePeople;
        }

        public override void BroadcastPlacement()
        {
            Locator.MessageHub.QueueMessage(MessageName, new PopContainerSetMessageArgs{ PopContainerSet = this });
        }

        private void UpdatePeople()
        {
            Containers.ForEach(i => i.ApplyAffectors());
        }

        public override string NodeName { get { return "PopContainerSet"; } }
    }
}