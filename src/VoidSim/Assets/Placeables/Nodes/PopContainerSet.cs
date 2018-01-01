using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.UI;
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
        [SerializeField] private TimeLength _updateTime;
        [SerializeField] private PopContainerSetViewModel _viewModelPrefab;
        private PopContainerSetViewModel _viewModel;

        protected override PopContainerSet GetThis() { return this; }
        public const string MessageName = "PopContainerNodeCreated";

        private readonly List<PopContainer> _containers = new List<PopContainer>();

        public Action<List<PopContainer>> OnContainersUpdated;

        // creates a container, adds it to the list, returns it
        public PopContainer CreateContainer(PopContainerParams param)
        {
            var container = new PopContainer(param);
            container.OnUpdate += () =>
            {
                if (OnContainersUpdated != null) OnContainersUpdated(_containers);
            };
            if (param.Type == PopContainerType.Employment)
            {
                _employmentContainer = container;
            }
            else if (param.Type == PopContainerType.Service)
            {
                _serviceContainer = container;
            }
            _containers.Add(container);

            if (OnContainersUpdated != null) OnContainersUpdated(_containers);
            return container;
        }

        public override void BroadcastPlacement()
        {
            var time = Locator.WorldClock.GetSeconds(_updateTime);
            var node = StopWatch.AddNode("apply_affectors", time);
            node.OnTick += UpdatePeople;

            CreateUi();

            Locator.MessageHub.QueueMessage(MessageName, new PopHolderMessageArgs{ PopContainerSet = this });
        }

        private void CreateUi()
        {
            var canvas = GameObject.Find("GameUICanvas");
            _viewModel = Instantiate(_viewModelPrefab, canvas.transform);
            _viewModel.Initialize(this);
        }

        private void UpdatePeople()
        {
            _containers.ForEach(i => i.ApplyAffectors());
        }

        public override string NodeName { get { return "PopContainerSet"; } }
    }
}