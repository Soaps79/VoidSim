using System.Collections.Generic;
using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using DG.Tweening;
using Messaging;
using QGame;
using UnityEngine;
#pragma warning disable 649

namespace Assets.Station.Population
{
    public class PersonSelectedMessageArgs : MessageArgs
    {
        public static string MessageName = "PersonSelected";
        public Person Person;
        public bool IsSelected;
    }

    /// <summary>
    /// Monitors all pop containers, references stored within as they are placed in-game
    /// Handles instantiating their UI components, and managing selection behavior for Persons
    /// </summary>
    public class OccupationMonitor : QScript, IMessageListener
    {
        [SerializeField] private PopContainerSetViewModel _viewModelPrefab;
        [SerializeField] private CanvasGroup _canvasGroupPrefab;
        [SerializeField] private bool _startHidden;
        private bool _isVisible;

        private CanvasGroup _canvasGroup;
        [SerializeField] private readonly List<PopContainerSetViewModel> _containers 
            = new List<PopContainerSetViewModel>();

        private Person _currentSelected;

        void Start()
        {
            var canvasTransform = Locator.CanvasManager.GetCanvas(CanvasType.Occupancy).transform;
            _canvasGroup = Instantiate(_canvasGroupPrefab, canvasTransform);
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);
            if(!_startHidden)
                Show();
            
            Locator.MessageHub.AddListener(this, PopContainerSet.MessageName);
            Locator.MessageHub.AddListener(this, PersonSelectedMessageArgs.MessageName);
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

            if (type == PersonSelectedMessageArgs.MessageName && args != null)
                HandleSelection(args as PersonSelectedMessageArgs);
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

        // Person UI selection works as follows:
        // Selection made in Occupancy UI - PersonSelected message sent and recieved here
        // Containers notified to make sure it is the only one selected
        // Monitor hooks into person (and saves into _currentSelected) to know when they move
        // When person moves, HandleSelectedPersonLocationChange is called, which triggers another container notify
        // when person panel is closed, monitor unhooks from _currentSelected, and notifies containers to deselect all
        private void HandleSelection(PersonSelectedMessageArgs args)
        {
            if(args == null)
                throw new UnityException("PersonSelectedMessage fired with bad data");

            // disconnect and null current selection
            if (_currentSelected != null)
            {
                _currentSelected.OnLocationChange -= HandleSelectedPersonLocationChange;
                _currentSelected = null;
            }

            // hook into new person if there is one
            if (args.IsSelected)
            {
                _currentSelected = args.Person;
                _currentSelected.OnLocationChange += HandleSelectedPersonLocationChange;
            }

            // flush the container UI's
            _containers.ForEach(i => i.DeselectExcept(_currentSelected));
        }

        private void HandleSelectedPersonLocationChange(Person person)
        {
            // if passing null, containers will deselect all
            _containers.ForEach(i => i.DeselectExcept(person));
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