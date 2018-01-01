using System.Collections.Generic;
using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station.Population
{
    /// <summary>
    /// Monitors all pop containers
    /// Stores references to containers as they are placed, handles instantiating their UI components
    /// </summary>
    public class OccupationMonitor : QScript, IMessageListener
    {
        [SerializeField] private PopContainerSetViewModel _viewModelPrefab;
        [SerializeField] private CanvasGroup _canvasGroupPrefab;
        private CanvasGroup _canvasGroup;
        private readonly List<PopContainerSetViewModel> _containers = new List<PopContainerSetViewModel>();

        void Start()
        {
            var canvasTransform = GameObject.Find("GameUICanvas").transform;
            _canvasGroup = Instantiate(_canvasGroupPrefab, canvasTransform);
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopContainerSet.MessageName && args != null)
                HandleContainer(args as PopContainerSetMessageArgs);
        }

        private void HandleContainer(PopContainerSetMessageArgs args)
        {
            if (args == null || args.PopContainerSet == null)
                throw new UnityException("PopContainerMonitor given bad message data");

            var viewModel = Instantiate(_viewModelPrefab, _canvasGroup.transform);
            viewModel.Initialize(args.PopContainerSet);
            _containers.Add(viewModel);
            args.PopContainerSet.OnRemove += set => OnRemove(viewModel);
        }

        private void OnRemove(PopContainerSetViewModel viewModel)
        {
            _containers.Remove(viewModel);
            Destroy(viewModel.gameObject);
        }

        public string Name { get { return "OccupationMonitor"; } }
    }
}