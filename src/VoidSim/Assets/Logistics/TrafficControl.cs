using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Logistics.Transit;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	public class TrafficControlData
	{
		public List<ShipBerthData> Berths;
	}

    [RequireComponent(typeof(TransitLocation))]
    [RequireComponent(typeof(ShipHolder))]
    // Handles ships in the Station's airspace
    // Give ships waypoints for their approach and departure
	public class TrafficControl : QScript, IMessageListener, ILocationDriver, ISerializeData<TrafficControlData>
	{
		public string ClientName { get { return "Station"; } }

		public Transform EntryLeft;
		public Transform EntryRight;
		public float VarianceY;

		private readonly List<ShipBerth> _berths = new List<ShipBerth>();
		// do something with this
		private readonly Queue<Ship> _queuedShips = new Queue<Ship>();
		private readonly List<Ship> _shipsInTraffic = new List<Ship>();

		// this class deals with persistence by holding onto the data,
		// and feeding it to the berths as they are placed
		private List<ShipBerthData> _deserialized = new List<ShipBerthData>();


		private readonly CollectionSerializer<TrafficControlData> _serializer
			= new CollectionSerializer<TrafficControlData>();

	    private TransitLocation _location;

	    void Start()
		{

		    BindToLocation();
		    Locator.MessageHub.AddListener(this, LogisticsMessages.ShipBerthsUpdated);

			if (_serializer.HasDataFor(this, "TrafficControl"))
				Load();
		}

        // Ships will arrive at _location, which this class is driving
	    private void BindToLocation()
	    {
	        _location = GetComponent<TransitLocation>();
	        _location.RegisterDriver(this);
	        _location.OnResume += Resume;
	    }

	    public void Load()
		{
			var data = _serializer.DeserializeData();
			_deserialized.AddRange(data.Berths);
		}

        // Finds the first empty berth of the ship's size and pairs them up
		private void FindBerthAndAssignToShip(Ship ship)
		{
			var berth = _berths.FirstOrDefault(i => i.ShipSize == ship.Size && !i.IsInUse);
			if (berth == null)
			{
				_queuedShips.Enqueue(ship);
				return;
			}

			berth.State = BerthState.Reserved;
			var waypoints = GenerateWayPoints(berth);
			ship.BeginHold(berth, waypoints);
			ship.TrafficShip.transform.SetParent(transform, true);
			_shipsInTraffic.Add(ship);
		}

		private List<Vector3> GenerateWayPoints(ShipBerth berth)
		{
			var list = new List<Vector3>();
			var rand = Random.value;
			list.Add(rand > .5
				? GenerateEntryPosition(EntryLeft.position)
				: GenerateEntryPosition(EntryRight.position));

			list.Add(berth.transform.position);

			list.Add(rand < .5
				? GenerateEntryPosition(EntryLeft.position)
				: GenerateEntryPosition(EntryRight.position));

			return list;
		}

		private Vector3 GenerateEntryPosition(Vector3 origin)
		{
			var start = origin;
			var variance = Random.Range(0, VarianceY);
			start = new Vector3(
				start.x,
				Random.value > .5 ? start.y + variance : start.y - variance, 0);
			return start;
		}

		public void Resume(Ship ship)
		{
			if (ship.Status == ShipStatus.Traffic && ship.TrafficShip != null )
			{
				ship.TrafficShip.transform.SetParent(transform, true);
				var berth = _berths.FirstOrDefault(i => i.name == ship.TrafficShip.BerthName);
				if (berth != null)
				{
					berth.Resume(ship.TrafficShip);
					ship.TrafficShip.Resume(berth);
				}
			}
		}

		public bool IsSimpleHold { get { return !_berths.Any(); } }

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.ShipBerthsUpdated && args != null)
				HandleBerthsUpdate(args as ShipBerthsMessageArgs);
		}

		private void HandleBerthsUpdate(ShipBerthsMessageArgs args)
		{
			if(args == null || !args.Berths.Any())
				throw new UnityException("TrafficControl given bad berths message");

			args.ShipBay.OnRemove += HandleShipBayRemove;
			AddBerths(args.Berths);
		}

		private void HandleShipBayRemove(ShipBay shipBay)
		{
			var names = shipBay.Berths.Select(i => i.name);
			var berths = _berths.Where(i => names.Contains(i.name)).ToList();

			foreach (var shipBerth in berths)
			{
				var ship = _shipsInTraffic.FirstOrDefault(
					i => i.TrafficShip.BerthName == shipBerth.name 
					&& (i.TrafficShip.Phase == TrafficPhase.Approaching || i.TrafficShip.Phase == TrafficPhase.Docked));
				if(ship != null)
					ship.TrafficShip.BeginEarlyDeparture();

				_berths.Remove(shipBerth);
			}
		}

		private void AddBerths(List<ShipBerth> berths)
		{
			var isFirst = !_berths.Any();
			foreach (var berth in berths)
			{
				_berths.Add(berth);

				if (_deserialized.Any())
				{
					var toFind = _deserialized.FirstOrDefault(i => i.Name == berth.name);
					if (toFind != null)
						berth.SetFromData(toFind);
					_deserialized.Remove(toFind);
				}
			}
            // this will work for the first placement of berths when ships are on hold, 
            // but will need love to work for arrivals > berth count queueing
			if (isFirst && _location.HasShipsOnHold())
			{
			    var count = _berths.Count(i => !i.IsInUse);
			    var ships = _location.GetShipsFromHold(count);
                ships.ForEach(FindBerthAndAssignToShip);
			}
		}

		public string Name { get { return "TrafficControl"; } }
		public TrafficControlData GetData()
		{
			return new TrafficControlData
			{
				Berths = _berths.Select(i => i.GetData()).ToList()
			};
		}

	    public bool TryHandleArrival(Ship ship)
	    {
	        if (_berths.All(i => i.IsInUse))
	            return false;

            FindBerthAndAssignToShip(ship);
	        return true;
	    }
	}
}