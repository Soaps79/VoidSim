using Assets.Logistics.Ships;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	/// <summary>
	/// Handles creation of ships, initializing their navigation
	/// </summary>
	[RequireComponent(typeof(TransitRegister))]
	public class TransitGenerator : QScript, IMessageListener
	{
		private int _lastManifestId;
		private int _lastShipId;
		private TransitRegister _transitRegister;
		[SerializeField] private GameObject _cargoShip;
		[SerializeField] private ShipSchedule _schedule;

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipCreated);
			_transitRegister = gameObject.GetComponent<TransitRegister>();
			
			// give locations some time to register
			var node = StopWatch.AddNode("begin", 1, true);
			node.OnTick += InitializeShips;
		}

		private void InitializeShips()
		{
			var locations = _transitRegister.GetTransitLocations();
			if(locations.Count < 2) throw new UnityException("TransitGenerator found less than 2 locations");

			if (_schedule == null)
			{
				Debug.Log("TransitGenerator has no ship schedule");
				return;
			}

			foreach (var entry in _schedule.Entries)
			{
				_lastShipId++;
				var ship = new Ship{ Name = "ship_" + _lastShipId };
				var navigation = new ShipNavigation { ParentShip = ship };
				locations.ForEach(i => navigation.AddLocation(i));
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
			args.Ship.CompleteVisit();
		}

		public string Name { get { return name; } }
	}
}