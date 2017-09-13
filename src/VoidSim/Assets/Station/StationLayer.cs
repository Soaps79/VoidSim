using System.Collections.Generic;
using Assets.Placeables;
using Assets.Scripts;
using Assets.WorldMaterials;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class StationLayer : QScript, IMessageListener
	{
		private Station _parentStation;
		private Inventory _inventory;
		public LayerType LayerType;
		private IHardPointManager _hardPoints;

		public void Initialize(Station parentStation, Inventory inventory)
		{
			_parentStation = parentStation;
			_inventory = inventory;
			var hardpoints = GetComponentInChildren<HardPointManager>();
			if (hardpoints != null)
			{
				hardpoints.Initialize(LayerType);
				_hardPoints = hardpoints;
			}
			else
				_hardPoints = new NullHardpointManager();
		}

		// Use this for initialization
		void Start ()
		{
			Locator.MessageHub.AddListener(this, PlaceableMessages.PlaceablePlaced);
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == PlaceableMessages.PlaceablePlaced)
				HandlePlaceableUpdate(args as PlaceableUpdateArgs);
		}

		private void HandlePlaceableUpdate(PlaceableUpdateArgs placed)
		{
			if (placed == null)
				throw new UnityException("StationLayer recieved bad placeable data");

			switch (placed.State)
			{
				case PlaceablePlacementState.BeginPlacement:
					HandleBeginPlacement(placed);
					break;
				case PlaceablePlacementState.Placed:
					HandlePlaced(placed);
					break;
			}
		}

		private void HandleBeginPlacement(PlaceableUpdateArgs placed)
		{
			if (placed.Layer == LayerType)
				_hardPoints.ActivateHardpoints();
		}

		private void HandlePlaced(PlaceableUpdateArgs placed)
		{
			_hardPoints.CompletePlacement();
			if (placed.Layer == LayerType)
				placed.Placeable.transform.SetParent(transform);
		}

		public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
	}
}
