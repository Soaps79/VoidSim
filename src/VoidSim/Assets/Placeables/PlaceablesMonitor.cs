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
		private List<PlaceableData> _placeables = new List<PlaceableData>();
		private const string _collectionName = "Placeables";

		void Start()
		{
			MessageHub.Instance.AddListener(this, PlaceableMessages.PlaceablePlaced);
			MessageHub.Instance.AddListener(this, GameMessages.PreSave);

			if (SerializationHub.Instance.IsLoading)
				HandleLoadingPlaceables();
		}

		// Loads by name the placeable from the lookup, and re-places it
		private void HandleLoadingPlaceables()
		{
			var raw = SerializationHub.Instance.GetCollection(_collectionName);
			var data = JsonConvert.DeserializeObject<PlaceablesMonitorData>(raw);
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

			else if (type == GameMessages.PreSave)
				HandlePreSave();
		}

		private void HandlePlaceablePlaced(PlaceablePlacedArgs args)
		{
			_placeables.Add(new PlaceableData { Name = args.ObjectPlaced.Name, Position = args.ObjectPlaced.transform.position });
		}

		private void HandlePreSave()
		{
			SerializationHub.Instance.AddCollection(_collectionName, GetData());
		}

		public string Name { get; private set; }
		public PlaceablesMonitorData GetData()
		{
			return new PlaceablesMonitorData{ Placeables = _placeables };
		}
	}
}