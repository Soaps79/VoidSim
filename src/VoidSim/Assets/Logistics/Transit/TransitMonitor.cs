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

namespace Assets.Logistics.Transit
{
	public class TransitMonitorData
	{
		public List<ShipData> Ships;
	}

	/// <summary>
	/// This class knows of all entities using the transit system. It is responsible for:
	/// Persisting any objects in transit
	/// Distributing Cargo requests
	/// </summary>
	public class TransitMonitor : QScript, IMessageListener, ISerializeData<TransitMonitorData>
	{
		private readonly List<Ship> _ships = new List<Ship>();
		[SerializeField] private TransitControl _transitControl;
		[SerializeField] private TrafficControl _trafficControl;

		[SerializeField] private GameObject _cargoShip;
		public Action<Ship> OnShipAdded;
		private readonly List<CargoManifest> _manifestsBacklog = new List<CargoManifest>();

		private readonly CollectionSerializer<TransitMonitorData> _serializer
			= new CollectionSerializer<TransitMonitorData>();

		void Start()
		{
			Locator.MessageHub.AddListener(this, LogisticsMessages.ShipCreated);
			Locator.MessageHub.AddListener(this, LogisticsMessages.CargoRequested);
			BindToUI();
			var node = StopWatch.AddNode("check_backlog", 5);
			node.OnTick += HandleManifestsBacklog;
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

				var lookup = ScriptableObject.Instantiate(Resources.Load("Ships/ship_lookup")) as ShipSOLookup;
				if (lookup == null)
					throw new UnityException("TransitMonitor could not find ship lookup");

				var scriptable = lookup.GetShips().FirstOrDefault(i => i.name == shipData.SOName);
				if (scriptable == null)
					throw new UnityException(string.Format("ShipGenerator cannot find SO named {0}", shipData.SOName));

				ship.SetScriptable(scriptable);
				ship.Initialize(navigation, shipData);

				if (ship.Status == ShipStatus.Hold || ship.Status == ShipStatus.Traffic)
				{
					var loc = locations.FirstOrDefault(i => i.ClientName == ship.Navigation.CurrentDestination);
					loc.Resume(ship);
				}

				Locator.MessageHub.QueueMessage(
					LogisticsMessages.ShipCreated, new ShipCreatedMessageArgs { Ship = ship, IsExisting = true });
			}
		}

		private void HandleManifestsBacklog()
		{
			if (!_manifestsBacklog.Any())
				return;

			var manifests = _manifestsBacklog.ToList();
			foreach (var manifest in manifests)
			{
				var ship = CargoCarrierFinder.FindCarrier(_ships, manifest);
				if (ship == null)
					continue;
				ship.AddManifest(manifest);
				_manifestsBacklog.Remove(manifest);
			}
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.ShipCreated && args != null)
				HandleShipCreated(args as ShipCreatedMessageArgs);

			else if (type == LogisticsMessages.CargoRequested && args != null)
				HandleCargoRequested(args as CargoRequestedMessageArgs);
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
			if (OnShipAdded != null)
				OnShipAdded(ship);
		}

		private void HandleCargoRequested(CargoRequestedMessageArgs args)
		{
			if (args == null)
				throw new UnityException("TransitMonitor recieved bad cargo request args");

			args.Manifest.Id = Locator.LastId.GetNext("manifest");

			var ship = CargoCarrierFinder.FindCarrier(_ships, args.Manifest);
			if (ship == null)
				_manifestsBacklog.Add(args.Manifest);
			else
			{
				ship.AddManifest(args.Manifest);
			}
		}

		private void BindToUI()
		{
			var go = (GameObject)Instantiate(Resources.Load("Views/transit_monitor_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "transit_monitor_viewmodel";
			var viewmodel = go.GetOrAddComponent<TransitMonitorViewModel>();
			viewmodel.Initialize(this, _transitControl);
		}

		public string Name { get { return "TransitMonitor"; } }
		public TransitMonitorData GetData()
		{
			return new TransitMonitorData {Ships = _ships.Select(i => i.GetData()).ToList()};
		}
	}
}