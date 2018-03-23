using System.Collections.Generic;
using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.Scripts.UI;
using DG.Tweening;
using Messaging;
using QGame;
using UnityEngine;
#pragma warning disable 649

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
        [SerializeField] private bool _startHidden;
        private bool _isVisible;

        private CanvasGroup _canvasGroup;
        [SerializeField] private List<PopContainerSetViewModel> _containers 
            = new List<PopContainerSetViewModel>();

        void Start()
        {
            var canvasTransform = Locator.CanvasManager.GetCanvas(CanvasType.Occupancy).transform;
            _canvasGroup = Instantiate(_canvasGroupPrefab, canvasTransform);
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);
            if(!_startHidden)
                Show();
            
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
            OnEveryUpdate += CheckForKeypress;
        }

        private void CheckForKeypress()
        {
            if (Input.GetButtonDown("Population Display"))
            {
                if(_isVisible)
                    Hide();
                else
                    Show();
            }
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
            viewModel.name = args.PopContainerSet.name;
            viewModel.Initialize(args.PopContainerSet);
            _containers.Add(viewModel);
            args.PopContainerSet.OnRemove += set => OnRemove(viewModel);
        }

        private void OnRemove(PopContainerSetViewModel viewModel)
        {
            _containers.Remove(viewModel);
            Destroy(viewModel.gameObject);
        }

        private void Show()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOFade(1, .5f);
            _isVisible = true;
        }

        private void Hide()
        {
            _canvasGroup.DOFade(0, .5f)
                .OnComplete(() => _canvasGroup.gameObject.SetActive(false));
            _isVisible = false;
        }

        public string Name { get { return "OccupationMonitor"; } }
    }
}