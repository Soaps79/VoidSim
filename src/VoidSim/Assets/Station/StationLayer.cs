using System.Collections.Generic;
using Assets.Placeables;
using Assets.Placeables.HardPoints;
using Assets.Scripts;
using Assets.WorldMaterials;
using DG.Tweening;
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
		private IHardPointGroup _hardPoints;
		private SpriteRenderer _sprite;
		private bool _isFaded;

		public void Initialize(Station parentStation, Inventory inventory)
		{
			_parentStation = parentStation;
			_inventory = inventory;
			_sprite = GetComponent<SpriteRenderer>();
			InitializeHardpoints();
		}

		// find and initialize the hardpoint manager, or instantiate a null one
		private void InitializeHardpoints()
		{
			var hardpoints = GetComponentInChildren<HardPointGroup>();
			if (hardpoints == null)
			{
				_hardPoints = new NullHardpointGroup();
				return;
			}

			hardpoints.Initialize(LayerType);
			_hardPoints = hardpoints;
			Locator.MessageHub.QueueMessage(HardPointGroup.MessageName, 
				new HardPointGroupUpdateMessage
				{
					Group =  hardpoints,
					Layer = LayerType
				});
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
				case PlaceablePlacementState.Cancelled:
					CompletePlacement();
					break;
			}
		}

		private void HandleBeginPlacement(PlaceableUpdateArgs placed)
		{
			if (placed.Layer != LayerType && _sprite != null)
			{
				var color = _sprite.color;
				color.a = .5f;
				_sprite.DOColor(color, .3f);
				_isFaded = true;
			}
		}

		private void HandlePlaced(PlaceableUpdateArgs placed)
		{
			if (placed.Layer == LayerType)
				placed.Placeable.transform.SetParent(transform);

			CompletePlacement();
		}

		private void CompletePlacement()
		{
			if (_isFaded)
			{
				var color = _sprite.color;
				color.a = 1f;
				_sprite.DOColor(color, .3f);
			}
		}

		public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
	}
}
