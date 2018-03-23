using System.Collections.Generic;
using Assets.Controllers.GUI;
using Assets.Placeables;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PlaceablePanelManager : QScript
    {
        private readonly Dictionary<string, PlaceableViewModel> _activeViewModels
            = new Dictionary<string, PlaceableViewModel>();

        private PlaceablesLookup _scriptable;
        private Transform _centerPoint;

        /// <summary>
        /// Must be called once in order for object to function
        /// </summary>
        void Start()
        {
            var station = GameObject.Find("Station");
            _centerPoint = station.transform;
            _scriptable = Instantiate(Resources.Load("placeables_lookup")) as PlaceablesLookup;
        }

        public void AddPanel(Placeable placeable)
        {
            if (!_activeViewModels.ContainsKey(placeable.name))
                EnableUi(placeable);
            else
                DisableUi(placeable.name);
        }

        // create the UI item
        private void EnableUi(Placeable placeable)
        {
            // create view model
            var canvas = Locator.CanvasManager.GetCanvas(CanvasType.ConstantUpdate);
            var viewModelInstance = Instantiate(_scriptable.ViewModel, canvas.transform, false);
            viewModelInstance.Bind(placeable);
            var position = GetUiObjectPosition(placeable.transform);
            viewModelInstance.transform.position = position;
            _activeViewModels.Add(placeable.name, viewModelInstance);

            var close = viewModelInstance.GetComponent<ClosePanelButton>();
            if (close != null)
                close.OnClose += () => { DisableUi(placeable.name); };
        }

        private Vector2 GetUiObjectPosition(Transform placeableTransform)
        {
            const float distance = 40f;
            var placeablePoint = placeableTransform.position;
            var stationPoint = _centerPoint.position;

            var direction = placeablePoint - stationPoint;
            var offset = direction.normalized * distance;

            return stationPoint + offset;
        }

        // destroy the UI item
        private void DisableUi(string placeableName)
        {
            if (!_activeViewModels.ContainsKey(placeableName))
                return;

            var viewModel = _activeViewModels[placeableName];
            Destroy(viewModel.gameObject);
            _activeViewModels.Remove(placeableName);
        }
    }
}