using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using Newtonsoft.Json;
using QGame;
using UnityEngine;

namespace Assets.Placeables
{
	public class PlaceableData
	{
		public string Name;
		public Vector3Data Position;
	}

	public class PlaceablesMonitorData
	{
		public List<PlaceableData> Placeables;
	}

	/// <summary>
	/// This object is responsible for tracking and persisting all placed Placeables.
	/// </summary>
	public class PlaceablesMonitor : QScript, IMessageListener, ISerializeData<PlaceablesMonitorData>
	{
		private readonly List<PlaceableData> _placeables = new List<PlaceableData>();

		private readonly CollectionSerializer<PlaceablesMonitorData> _serializer 
			= new CollectionSerializer<PlaceablesMonitorData>();

		void Start()
		{
			MessageHub.Instance.AddListener(this, PlaceableMessages.PlaceablePlaced);

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
				var scriptable = placeablesLookup.Placeables.FirstOrDefault(i => i.ProductName == placeableData.Name);
				if(scriptable != null)
					Placer.PlaceObject(scriptable, placeableData.Position);
			}
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == PlaceableMessages.PlaceablePlaced && args != null)
				HandlePlaceablePlaced(args as PlaceablePlacedArgs);
		}

		private void HandlePlaceablePlaced(PlaceablePlacedArgs args)
		{
			_placeables.Add(new PlaceableData { Name = args.ObjectPlaced.Name, Position = args.ObjectPlaced.transform.position });
		}

		public string Name { get; private set; }
		public PlaceablesMonitorData GetData()
		{
			return new PlaceablesMonitorData{ Placeables = _placeables };
		}
	}
}