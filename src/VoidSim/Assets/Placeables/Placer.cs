using System;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Placeables
{
	public class Placer : QScript
	{
		private GameObject _toPlaceGo;
		private PlaceableScriptable _toPlaceScriptable;
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
				HandlePlaceObject();
			}
			else if (Input.GetButtonDown("Cancel"))
			{
				CancelPlacement();
			}
		}

		private void HandlePlaceObject()
		{
			PlaceObject(_toPlaceScriptable, _toPlaceGo.transform.position);
			CompletePlacement(true);
		}

		public static void PlaceObject(PlaceableScriptable scriptable, Vector3 position, PlaceableData data = null)
		{
			var placeable = Instantiate(scriptable.Prefab);
			placeable.transform.position = position;
			placeable.BindToScriptable(scriptable);
			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.Placed,
					Placeable = placeable,
					Layer = scriptable.Layer
				});
			placeable.InitializeNodes(data);
		}

		private void CancelPlacement()
		{
			CompletePlacement(false);
		}

		private void CompletePlacement(bool wasPlaced)
		{
			if (OnPlacementComplete != null)
				OnPlacementComplete(wasPlaced ? _toPlaceInventoryId : 0);

			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.Cancelled,
					Layer = _toPlaceScriptable.Layer
				});

			_toPlaceInventoryId = 0;
			Destroy(_toPlaceGo);
			_toPlaceGo = null;
			_toPlaceScriptable = null;
			enabled = false;
		}

		public void BeginPlacement(string placeableName, int inventoryId)
		{
			var placeable = _lookup.Placeables.FirstOrDefault(i => i.ProductName == placeableName);
			if(placeable == null)
				throw new UnityException(string.Format("Placer asked to place {0}: Has no entry in PlaceablesLookup", placeableName));

			_toPlaceInventoryId = inventoryId;
			_toPlaceScriptable = placeable;
			var go = new GameObject();
			var rend = go.AddComponent<SpriteRenderer>();
			rend.sprite = placeable.PlacedSprite;
			rend.sortingLayerName = placeable.Layer.ToString();
			rend.sortingOrder = 1;
			_toPlaceGo = go;

			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.BeginPlacement,
					Layer = placeable.Layer
				});

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