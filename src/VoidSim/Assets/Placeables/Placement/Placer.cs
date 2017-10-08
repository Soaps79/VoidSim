using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.HardPoints;
using Assets.Scripts;
using QGame;
using UnityEngine;

namespace Assets.Placeables.Placement
{
	/// <summary>
	/// Handles adding placeables to the scene.
	/// Used by InventoryViewModel when user is placing object.
	/// Also presents static function to use in game loading.
	/// </summary>
	[RequireComponent(typeof(HardPointMagnet))]
	public class Placer : QScript
	{
		private GameObject _toPlaceGo;
		private PlaceableScriptable _toPlaceScriptable;
		private int _toPlaceInventoryId;
		private PlaceablesLookup _lookup;

		public Action<int> OnPlacementComplete;
		private HardPointMagnet _magnet;
		private static string _hardpointName; // hacky, I know. refactor if extracting static PlaceObject

		public void Initialize(PlaceablesLookup placeables, HardPointMonitor hardPointMonitor)
		{
			_lookup = placeables;
			_magnet = gameObject.GetComponent<HardPointMagnet>();
			_magnet.Initialize(hardPointMonitor);

			OnEveryUpdate += CheckForKeyPress;
			enabled = false;
		}

		// handles user placing object or dismissing it
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

		// if object is on hard point, place it
		private void HandlePlaceObject()
		{
			if (!_magnet.CanPlace())
				return;

			_hardpointName = _magnet.SnappedTo.name;
			PlaceObject(_toPlaceScriptable, _toPlaceGo.transform.position);
			CompletePlacement(true);
		}

		// static function also used on game load, can be moved elsewhere if need be
		public static void PlaceObject(PlaceableScriptable scriptable, Vector3 position, PlaceableData data = null)
		{
			var placeable = Instantiate(scriptable.Prefab);
			placeable.HardPointName = data == null ? _hardpointName : data.HardPointName;
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

		// placement has been dismissed by user
		private void CancelPlacement()
		{
			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.Cancelled,
					Layer = _toPlaceScriptable.Layer
				});
			CompletePlacement(false);
		}

		// turn off the magnet, null all the things
		private void CompletePlacement(bool wasPlaced)
		{
			if (OnPlacementComplete != null)
				OnPlacementComplete(wasPlaced ? _toPlaceInventoryId : 0);

			_magnet.Complete();

			_toPlaceInventoryId = 0;
			Destroy(_toPlaceGo);
			_toPlaceGo = null;
			_toPlaceScriptable = null;
			enabled = false;
		}

		// loads up a GO with placeable's sprite
		public void BeginPlacement(string placeableName, int inventoryId)
		{
			var placeable = _lookup.Placeables.FirstOrDefault(i => i.ProductName == placeableName);
			if(placeable == null)
				throw new UnityException(string.Format("Placer asked to place {0}: Has no entry in PlaceablesLookup", placeableName));

			_toPlaceInventoryId = inventoryId;
			_toPlaceScriptable = placeable;
			// this could be refactored to be a PlacementPlaceable
			// create a seam to place placeables in the editor using this object
			var go = new GameObject();
			var rend = go.AddComponent<SpriteRenderer>();
			rend.sprite = placeable.PlacedSprite;
			rend.sortingLayerName = placeable.Layer.ToString();
			rend.sortingOrder = 1;
			_toPlaceGo = go;

			// magnet handles positioning of placement sprite
			_magnet.Begin(_toPlaceGo, placeable.Layer);
			
			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.BeginPlacement,
					Layer = placeable.Layer
				});

			enabled = true;
		}
	}
}