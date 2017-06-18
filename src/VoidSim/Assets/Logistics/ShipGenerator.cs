using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	public class ShipGeneratorData
	{
		public List<QueuedShipData> Ships;
		public int LastId;
	}

	public class QueuedShipData
	{
		public string ShipName;
		public string SOName;
		public float TimeRemaining;
	}

	/// <summary>
	/// Handles creation of ships, initializing their navigation. Currently used to launch 
	/// a bunch of ships into the world from a starting list, will eventually make more sense
	/// </summary>
	[RequireComponent(typeof(TransitControl))]
	public class ShipGenerator : QScript, IMessageListener, ISerializeData<ShipGeneratorData>
	{
		// will eventually be used to store type of ship
		public class Entry
		{
			public string ShipName;
			public string SOName;
			public StopWatchNode Node;
		}

		private int _lastShipId;
		private TransitControl _transitControl;
		[SerializeField] private GameObject _cargoShip;
		[SerializeField] private ShipSchedule _schedule;

		private readonly CollectionSerializer<ShipGeneratorData> _serializer
			= new CollectionSerializer<ShipGeneratorData>();

		private readonly List<Entry> _toLaunch = new List<Entry>();
		private List<ShipSO> _shipSOs;

		void Start()
		{
			// currently listens to own message, and if ship is new, 'kicks' it
			// may not be needed anymore?
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			_transitControl = gameObject.GetComponent<TransitControl>();
			_shipSOs = ShipSOLookup.Instance.GetShips();

			if (_serializer.HasDataFor(this, "ShipGenerator"))
				LoadSerializedShips();
			else
				LoadInitialShips();
		}

		// pulls data from serializer and queues ships
		private void LoadSerializedShips()
		{
			var data = _serializer.DeserializeData();
			_lastShipId = data.LastId;
			foreach (var shipData in data.Ships)
			{
				QueueShip(shipData.ShipName, shipData.SOName, shipData.TimeRemaining);
			}
		}

		// pulls starting schedule from SO and queues ships
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
				QueueShip(shipName, entry.SOName, entry.InitialDelay);
			}
		}

		// begins a timer that will launch the ship when it is complete
		private void QueueShip(string shipName, string soName, float time)
		{
			var node = StopWatch.AddNode(shipName, time, true);
			var e = new Entry
			{
				ShipName = shipName,
				SOName = soName,
				Node = node
			};
			node.OnTick += () => CreateShip(e);
			_toLaunch.Add(e);
		}

		// fills a navigation object with all known locations and instantiates a ship
		private void CreateShip(Entry entry)
		{
			var locations = _transitControl.GetTransitLocations();
			if (locations.Count < 2) throw new UnityException("ShipGenerator found less than 2 locations");

			var ship = new Ship { Name = entry.ShipName };
			var scriptable = _shipSOs.FirstOrDefault(i => i.name == entry.SOName);
			if (scriptable == null)
				throw new UnityException(string.Format("ShipGenerator cannot find SO named {0}", entry.SOName));

			ship.SetScriptable(scriptable);
			var navigation = new ShipNavigation();
			locations.ForEach(i => navigation.AddLocation(i.ClientName));
			navigation.CycleLocations();
			ship.Initialize(navigation, _cargoShip);

			_toLaunch.Remove(entry);

			MessageHub.Instance.QueueMessage(LogisticsMessages.ShipCreated, new ShipCreatedMessageArgs { Ship = ship });
		}

		// not sure if still necessary to wait a cycle?
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

		public ShipGeneratorData GetData()
		{
			return new ShipGeneratorData
			{
				LastId = _lastShipId,
				Ships = _toLaunch.Select(i => new QueuedShipData
				{
					ShipName = i.ShipName,
					SOName = i.SOName,
					TimeRemaining = i.Node.RemainingLifetime
				}).ToList()
			};
		}
	}
}