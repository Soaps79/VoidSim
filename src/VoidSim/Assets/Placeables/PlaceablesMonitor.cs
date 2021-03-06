﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Placement;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Placeables
{
	public class PlaceablesMonitorData
	{
		public List<PlaceableData> Placeables;
	}

	/// <summary>
	/// This object is responsible for tracking and persisting all placed Placeables.
	/// </summary>
	public class PlaceablesMonitor : QScript, IMessageListener, ISerializeData<PlaceablesMonitorData>
	{
		public Transform CenterUI;

		private readonly List<Placeable> _placeables = new List<Placeable>();

		private readonly CollectionSerializer<PlaceablesMonitorData> _serializer 
			= new CollectionSerializer<PlaceablesMonitorData>();

		public Action<Placeable, PlaceablePlacementState> OnPlaced;

		void Start()
		{
			Locator.MessageHub.AddListener(this, PlaceableMessages.PlaceablePlaced);

			if(_serializer.HasDataFor(this, "Placeables"))
				HandleLoadingPlaceables();
		}

		// Loads by name the placeable from the lookup, and re-places it
		private void HandleLoadingPlaceables()
		{
			var data = _serializer.DeserializeData();
			var placeablesLookup = Instantiate(Resources.Load("placeables_lookup")) as PlaceablesLookup;

			foreach (var placeableData in data.Placeables)
			{
				var scriptable = placeablesLookup.Placeables.FirstOrDefault(i => i.ProductName == placeableData.PlaceableName);
				if(scriptable != null)
					Placer.PlaceObject(scriptable, placeableData.Position, placeableData);
			}
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == PlaceableMessages.PlaceablePlaced && args != null)
				HandlePlaceableUpdate(args as PlaceableUpdateArgs);
		}

		private void HandlePlaceableUpdate(PlaceableUpdateArgs args)
		{
			if(args == null)
				throw new UnityException("PlaceablesMonitor given bad data");

			switch (args.State)
			{
				case PlaceablePlacementState.Placed:
					_placeables.Add(args.Placeable);
					break;
				case PlaceablePlacementState.Removed:
					if (!_placeables.Remove(args.Placeable))
						return;
					Destroy(args.Placeable.gameObject);
					break;
			}

			// follow this post adding remove
			if (OnPlaced != null)
				OnPlaced(args.Placeable, args.State);
		}

		public string Name { get { return "PlaceablesMonitor"; } }
		public PlaceablesMonitorData GetData()
		{
			return new PlaceablesMonitorData
			{
				Placeables = _placeables.Select(i => i.GetData()).ToList()
			};
		}
	}
}