﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Logistics.UI;
using Assets.Scripts;
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
		private TransitControl _control;

		public Action<Ship> OnShipAdded;
		private readonly List<CargoManifest> _manifestsBacklog = new List<CargoManifest>();

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			MessageHub.Instance.AddListener(this, LogisticsMessages.CargoRequested);
			BindToUI();
			var node = StopWatch.AddNode("check_backlog", 5);
			node.OnTick += HandleManifestsBacklog;
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

			_lastManifestId++;
			args.Manifest.Id = _lastManifestId;

			var ship = CargoCarrierFinder.FindCarrier(_ships, args.Manifest);
			if (ship == null)
				_manifestsBacklog.Add(args.Manifest);
			else
			{
				ship.AddManifest(args.Manifest);
				var s = string.Format("{0} given manifest {1}", ship.Name, args.Manifest.Id);
				UberDebug.LogChannel(this, "Trade", s);
			}
		}

		private void BindToUI()
		{
			var go = (GameObject)Instantiate(Resources.Load("Views/transit_monitor_viewmodel"));
			go.transform.SetParent(transform);
			go.name = "transit_monitor_viewmodel";
			var viewmodel = go.GetOrAddComponent<TransitMonitorViewModel>();
			viewmodel.Initialize(this);

			KeyValueDisplay.Instance.Add("Manifest Backlog", () => _manifestsBacklog.Count);
		}

		public string Name { get { return "TransitMonitor"; } }
	}
}