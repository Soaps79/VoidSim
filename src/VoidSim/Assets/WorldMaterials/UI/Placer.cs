using System;
using System.Linq;
using Assets.Scripts;
using Assets.Station;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials.UI
{
    public class Placer : QScript
    {
        private GameObject _toPlaceGo;
        private PlaceableScriptable _toPlaceableScriptable;
        private int _toPlaceInventoryId;
        private PlaceablesLookup _lookup;

        public Action<int> OnPlacementComplete;

        public void Initialize(PlaceablesLookup placeables)
        {
            _lookup = placeables;
            OnEveryUpdate += BindSpritePositionToMouseCursor;
            OnEveryUpdate += CheckForKeyPress;
            enabled = false;
        }

        private void CheckForKeyPress(float obj)
        {
            if (Input.GetButtonDown("Confirm"))
            {
                PlaceObject();
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                CancelPlacement();
            }
        }

        private void PlaceObject()
        {
            var placeable = Instantiate(_toPlaceableScriptable.Prefab);
            placeable.transform.position = _toPlaceGo.transform.position;
            placeable.BindToScriptable(_toPlaceableScriptable);
            MessageHub.Instance.QueueMessage(
                PlaceableMessages.PlaceablePlacedMessageName, 
                new PlaceablePlacedArgs {ObjectPlaced = placeable});
            CompletePlacement(true);
        }
        
        private void CancelPlacement()
        {
            CompletePlacement(false);
        }

        private void CompletePlacement(bool wasPlaced)
        {
            if (OnPlacementComplete != null)
                OnPlacementComplete(wasPlaced ? _toPlaceInventoryId : 0);

            _toPlaceInventoryId = 0;
            Destroy(_toPlaceGo);
            _toPlaceGo = null;
            _toPlaceableScriptable = null;
            enabled = false;
        }

        public void BeginPlacement(string placeableName, int inventoryId)
        {
            var placeable = _lookup.Placeables.FirstOrDefault(i => i.ProductName == placeableName);
            if(placeable == null)
                throw new UnityException(string.Format("Placer asked to place {0}: Has no entry in PlaceablesLookup", placeableName));

            _toPlaceInventoryId = inventoryId;
            _toPlaceableScriptable = placeable;
            var go = new GameObject();
            var rend = go.AddComponent<SpriteRenderer>();
            rend.sprite = placeable.PlacedSprite;
            rend.sortingLayerName = placeable.Layer.ToString();
            _toPlaceGo = go;

            enabled = true;
        }

        private void BindSpritePositionToMouseCursor(float obj)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            _toPlaceGo.transform.position = mousePosition;
        }
    }
}