using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Logistics.UI;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;
#pragma warning disable 649

namespace Assets.Logistics.Transit
{
	public class TransitMonitorData
	{
		public List<ShipData> Ships;
	}

	/// <summary>
	/// This class knows of all entities using the transit system. It is responsible for:
	/// Persisting any objects in transit
	/// </summary>
	public class TransitMonitor : QScript, IMessageListener, ISerializeData<TransitMonitorData>
	{
		private readonly List<Ship> _ships = new List<Ship>();
		[SerializeField] private TransitControl _transitControl;
	    [SerializeField] private ShipSOLookup _shipLookup;
		public Action<Ship> OnShipAdded;

		private readonly CollectionSerializer<TransitMonitorData> _serializer
			= new CollectionSerializer<TransitMonitorData>();

		void Start()
		{
			Locator.MessageHub.AddListener(this, LogisticsMessages.ShipCreated);
			BindToUi();
			if (_serializer.HasDataFor(this, "ShipMonitor"))
				HandleGameLoad();
		}

		private void HandleGameLoad()
		{
			_ships.Clear();

			var data = _serializer.DeserializeData();
			// doing this to give transit locations, etc time to register
			StopWatch.AddNode("loadgame", .1f, true).OnTick += () => LoadShipsIntoGame(data);
		}

		private void LoadShipsIntoGame(TransitMonitorData data)
		{
			var locations = _transitControl.GetTransitLocations();
			foreach (var shipData in data.Ships)
			{
				var ship = new Ship { Name = shipData.Name };
				var navigation = new ShipNavigation(shipData.Navigation);

				if (_shipLookup == null)
					throw new UnityException("TransitMonitor missing its ship lookup");

				var scriptable = _shipLookup.GetShips().FirstOrDefault(i => i.name == shipData.SOName);
				if (scriptable == null)
					throw new UnityException($"TransitMonitor cannot find Ship SO named {shipData.SOName}");

				ship.SetScriptable(scriptable);
				ship.Initialize(navigation, shipData);

				if (ship.Status == ShipStatus.Hold || ship.Status == ShipStatus.Traffic)
				{
					var loc = locations.FirstOrDefault(i => i.ClientName == ship.Navigation.CurrentDestination);
                    if(loc == null)
                        throw new UnityException($"Ship loaded with unknown destination: {ship.Navigation.CurrentDestination}");
					loc.Resume(ship);
				}

				Locator.MessageHub.QueueMessage(
					LogisticsMessages.ShipCreated, new ShipCreatedMessageArgs { Ship = ship, IsExisting = true });
			}
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.ShipCreated && args != null)
				HandleShipCreated(args as ShipCreatedMessageArgs);
		}

		private void HandleShipCreated(ShipCreatedMessageArgs args)
		{
			if (args == null || args.Ship == null)
				throw new UnityException("TransitMonitor given bad ship created data");

			_ships.Add(args.Ship);
			CheckOnShipAdded(args.Ship);
		}

		private void CheckOnShipAdded(Ship ship)
		{
            OnShipAdded?.Invoke(ship);
        }

		private void BindToUi()
		{
			var go = (GameObject)Instantiate(Resources.Load("Views/transit_monitor_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "transit_monitor_viewmodel";
			var viewmodel = go.GetOrAddComponent<TransitMonitorViewModel>();
			viewmodel.Initialize(this);
		}

		public string Name => "TransitMonitor";

	    public TransitMonitorData GetData()
		{
			return new TransitMonitorData {Ships = _ships.Select(i => i.GetData()).ToList()};
		}
	}
}