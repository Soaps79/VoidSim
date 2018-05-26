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
    // Handles ships in the Station's airspace
    // Give ships waypoints for their approach and departure
	public class TrafficControl : QScript, IMessageListener, ILocationDriver, ISerializeData<TrafficControlData>
	{
		public string ClientName { get { return "Station"; } }

		public Transform EntryLeft;
		public Transform EntryRight;
	    [Tooltip("Largest possible Y offset from ship entry points")]
        public float VarianceY;

		private readonly List<ShipBerth> _berths = new List<ShipBerth>();

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
        // Assumes that the presence of an empty berth is already confirmed (will need love when diff ship sizes are put into play)
		private void FindBerthAndAssignToShip(Ship ship)
		{
			var berth = _berths.FirstOrDefault(i => i.ShipSize == ship.Size && !i.IsInUse);
			berth.State = BerthState.Reserved;
			var waypoints = GenerateWayPoints(berth);
			ship.BeginHold(berth, waypoints);
			ship.TrafficShip.transform.SetParent(transform, true);
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

        // called after game load, meaning ship already has its waypoints and is in motion
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

        // A ShipBay is a placeable with an array of Berths
        // Hook into Bay, add its berths, see if any ships are wiating for one
		private void HandleBerthsUpdate(ShipBerthsMessageArgs args)
		{
			if(args == null || !args.Berths.Any())
				throw new UnityException("TrafficControl given bad berths message");

			args.ShipBay.OnRemove += HandleShipBayRemove;
			AddBerths(args.Berths);
		}

		private void HandleShipBayRemove(ShipBay shipBay)
		{
            // using names is currently the most reliable way to match berths
			var names = shipBay.Berths.Select(i => i.name);
			var berths = _berths.Where(i => names.Contains(i.name)).ToList();

			foreach (var shipBerth in berths)
			{
                // if any ships are headed to the berth, tell them to leave
				var ship = _location.Ships.FirstOrDefault(
					i => i.TrafficShip.BerthName == shipBerth.name 
					&& (i.TrafficShip.Phase == TrafficPhase.Approaching || i.TrafficShip.Phase == TrafficPhase.Docked));
			    ship?.TrafficShip.BeginEarlyDeparture();
			    _berths.Remove(shipBerth);
			}
		}

        // Adds berths and checks if any ships are waiting for one
		private void AddBerths(IEnumerable<ShipBerth> berths)
		{
			var isFirst = !_berths.Any();
			foreach (var berth in berths)
			{
				_berths.Add(berth);

			    if (!_deserialized.Any()) continue;
			    var toFind = _deserialized.FirstOrDefault(i => i.Name == berth.name);
			    if (toFind != null)
			        berth.SetFromData(toFind);
			    _deserialized.Remove(toFind);
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

		public string Name => "TrafficControl";

	    public TrafficControlData GetData()
		{
			return new TrafficControlData
			{
				Berths = _berths.Select(i => i.GetData()).ToList()
			};
		}

        // if there is an open berth, send ship to it, otherwise let location put ship on hold
	    public bool TryHandleArrival(Ship ship)
	    {
	        if (_berths.All(i => i.IsInUse))
	            return false;

            FindBerthAndAssignToShip(ship);
	        return true;
	    }
	}
}