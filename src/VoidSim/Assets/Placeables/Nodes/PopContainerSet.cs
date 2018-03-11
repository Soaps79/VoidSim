using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopContainerSetMessageArgs : MessageArgs
    {
        public PopContainerSet PopContainerSet;
    }

    public class PopContainerSetData
    {
        public List<PopContainerData> Containers;
    }

    /// <summary>
    /// Provides containers for a Person's physical presence
    /// Placeables like the Lounge, where you will have both workers and patrons, require 2 groupings
    /// </summary>
    public class PopContainerSet : PlaceableNode<PopContainerSet>, ISerializeData<PopContainerSetData>
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

            if (_deserialized.ContainsKey(container.Name))
            {
                container.SetFromData(_deserialized[container.Name]);
                _deserialized.Remove(container.Name);
            }

            Containers.Add(container);

            if (OnContainersUpdated != null) OnContainersUpdated(Containers);
            return container;
        }

        private readonly Dictionary<string, PopContainerData> _deserialized = new Dictionary<string, PopContainerData>();

        public override void Initialize(PlaceableData data)
        {
            var time = Locator.WorldClock.GetSeconds(_updateTime);
            var node = StopWatch.AddNode("apply_affectors", time);
            node.OnTick += UpdatePeople;

            if (data != null && data.PopContainerData != null)
            {
                foreach (var popContainerData in data.PopContainerData.Containers)
                {
                    _deserialized.Add(popContainerData.Name, popContainerData);
                }
            }
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

        public override void AddToData(PlaceableData data)
        {
            data.PopContainerData = GetData();
        }

        public PopContainerSetData GetData()
        {
            return new PopContainerSetData
            {
                Containers = Containers.Select(i => i.GetData()).ToList()
            };
        }
    }
}