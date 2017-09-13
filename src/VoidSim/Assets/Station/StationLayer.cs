using System.Collections.Generic;
using Assets.Placeables;
using Assets.Scripts;
using Assets.WorldMaterials;
using Messaging;
using QGame;

namespace Assets.Station
{
	public class StationLayer : QScript, IMessageListener
	{
		private Station _parentStation;
		private Inventory _inventory;
		public LayerType LayerType;
		private HardPointManager _hardPoints;

		public void Initialize(Station parentStation, Inventory inventory)
		{
			_parentStation = parentStation;
			_inventory = inventory;
			_hardPoints = GetComponentInChildren<HardPointManager>();
		}

		// Use this for initialization
		void Start ()
		{
			Locator.MessageHub.AddListener(this, PlaceableMessages.PlaceablePlaced);
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == PlaceableMessages.PlaceablePlaced)
				HandlePlaceableAdd(args as PlaceableUpdateArgs);
		}

		private void HandlePlaceableAdd(PlaceableUpdateArgs placed)
		{
			if (placed == null || placed.Layer != LayerType || placed.State != PlaceablePlacementState.Placed)
				return;

			placed.Placeable.transform.SetParent(transform);
			_hardPoints.CompletePlacement();
		}

		public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
	}
}
