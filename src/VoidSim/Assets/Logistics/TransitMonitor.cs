using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Logistics.UI;
using Assets.Scripts;
using Assets.WorldMaterials.UI;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	/// <summary>
	/// This class knows of all entities using the transit system. It is responsible for:
	/// Persisting any objects in transit
	/// Distributing Cargo requests
	/// </summary>
	public class TransitMonitor : QScript, IMessageListener
	{
		private int _lastManifestId;
		private readonly List<Ship> _ships = new List<Ship>();
		private TransitRegister _register;

		public Action<Ship> OnShipAdded;

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			MessageHub.Instance.AddListener(this, LogisticsMessages.CargoRequested);
			BindToUI();
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

			_lastManifestId++;
			args.Manifest.Id = _lastManifestId;
			var ship = _ships.FirstOrDefault(i => i.Navigation.CurrentDestination.ClientName == "Void") ?? _ships[0];
			ship.AddManifest(args.Manifest);
			Debug.Log(string.Format("{0} given manifest {1}", ship.Name, args.Manifest.Id));
		}

		private void BindToUI()
		{
			var go = (GameObject)Instantiate(Resources.Load("Views/transit_monitor_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "transit_monitor_viewmodel";
			var viewmodel = go.GetOrAddComponent<TransitMonitorViewModel>();
			viewmodel.Initialize(this);
		}

		public string Name { get { return "TransitMonitor"; } }
	}
}