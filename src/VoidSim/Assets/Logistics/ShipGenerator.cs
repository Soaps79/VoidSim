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
		private int _lastManifestId;
		private int _lastShipId;
		private TransitControl _transitControl;
		[SerializeField] private GameObject _cargoShip;
		[SerializeField] private ShipSchedule _schedule;

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			_transitControl = gameObject.GetComponent<TransitControl>();

			// give locations some time to register

			if (!SerializationHub.Instance.IsLoading)
			{
				var node = StopWatch.AddNode("begin", 1, true);
				node.OnTick += InitializeShips;
			}
		}

		private void InitializeShips()
		{
			var locations = _transitControl.GetTransitLocations();
			if(locations.Count < 2) throw new UnityException("ShipGenerator found less than 2 locations");

			if (_schedule == null)
			{
				Debug.Log("ShipGenerator has no ship schedule");
				return;
			}

			foreach (var entry in _schedule.Entries)
			{
				_lastShipId++;
				var ship = new Ship{ Name = "ship_" + _lastShipId };
				var navigation = new ShipNavigation();
				locations.ForEach(i => navigation.AddLocation(i.ClientName));
				navigation.CycleLocations();
				ship.Initialize(navigation, _cargoShip);
				var node = StopWatch.AddNode(ship.Name, entry.InitialDelay, true);
				node.OnTick += () =>
				{
					MessageHub.Instance.QueueMessage(LogisticsMessages.ShipCreated, new ShipCreatedMessageArgs { Ship = ship });
				};
			}
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