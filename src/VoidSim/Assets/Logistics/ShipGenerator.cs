using System;
using System.Collections.Generic;
using Assets.Logistics.Ships;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	/// <summary>
	/// Handles creation of ships, initializing their navigation
	/// </summary>
	[RequireComponent(typeof(TransitControl))]
	public class ShipGenerator : QScript, IMessageListener
	{
		public class Entry
		{
			public string ShipName;
			public StopWatchNode Node;
		}

		private int _lastShipId;
		private TransitControl _transitControl;
		[SerializeField] private GameObject _cargoShip;
		[SerializeField] private ShipSchedule _schedule;

		private List<Entry> _toLaunch = new List<Entry>();

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			_transitControl = gameObject.GetComponent<TransitControl>();

			if (!SerializationHub.Instance.IsLoading)
			{
				// give locations some time to register
				var node = StopWatch.AddNode("begin", 1, true);
				node.OnTick += LoadInitialShips;
			}
		}

		private void LoadInitialShips()
		{
			if (_schedule == null)
			{
				Debug.Log("ShipGenerator has no ship schedule");
				return;
			}

			foreach (var entry in _schedule.Entries)
			{
				_lastShipId++;
				var shipName = "ship_" + _lastShipId;
				var node = StopWatch.AddNode(shipName, entry.InitialDelay, true);
				var e = new Entry
				{
					ShipName = shipName,
					Node = node
				};
				node.OnTick += () => CreateShip(e);
				_toLaunch.Add(e);
			}
		}

		private void CreateShip(Entry entry)
		{
			var locations = _transitControl.GetTransitLocations();
			if (locations.Count < 2) throw new UnityException("ShipGenerator found less than 2 locations");

			var ship = new Ship { Name = entry.ShipName };
			var navigation = new ShipNavigation();
			locations.ForEach(i => navigation.AddLocation(i.ClientName));
			navigation.CycleLocations();
			ship.Initialize(navigation, _cargoShip);

			_toLaunch.Remove(entry);

			MessageHub.Instance.QueueMessage(LogisticsMessages.ShipCreated, new ShipCreatedMessageArgs { Ship = ship });
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.ShipCreated && args != null)
			{
				OnNextUpdate += f => KickShip(args as ShipCreatedMessageArgs);
			}
		}

		private void KickShip(ShipCreatedMessageArgs args)
		{
			if(!args.IsExisting)
				args.Ship.CompleteVisit();
		}

		public string Name { get { return name; } }
	}
}