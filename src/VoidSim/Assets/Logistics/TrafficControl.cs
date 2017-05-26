using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
	public class TrafficControl : QScript, IMessageListener, ITransitLocation
	{
		public string ClientName { get { return "Station"; } }

		public Transform EntryLeft;
		public Transform EntryRight;
		public float VarianceY;

		private readonly List<ShipBerth> _berths = new List<ShipBerth>();
		// do something with this
		private readonly Queue<Ship> _queuedShips = new Queue<Ship>();
		private readonly List<Ship> _shipsInTraffic = new List<Ship>();

		// hold was originally used to handle arrivals before berths were placed
		// should also be used for when all berths are full
		[SerializeField] private ShipHolder _holder;

		void Start()
		{
			MessageHub.Instance.AddListener(this, LogisticsMessages.ShipBerthsUpdated);
			MessageHub.Instance.QueueMessage(LogisticsMessages.RegisterLocation, 
				new TransitLocationMessageArgs { TransitLocation = this });
		}

		public void OnTransitArrival(TransitControl.Entry entry)
		{
			if (!_berths.Any())
			{
				// this is temp, shouldn't be a thing eventually
				_holder.BeginHold(entry.Ship);
				Debug.Log("Ship arrived before berths placed");
			}

			FindBerthAndAssignToShip(entry.Ship);
		}

		private void FindBerthAndAssignToShip(Ship ship)
		{
			var berth = _berths.FirstOrDefault(i => i.ShipSize == ship.Size && !i.IsInUse);
			if (berth == null)
			{
				_queuedShips.Enqueue(ship);
				return;
			}

			berth.State = BerthState.Reserved;
			List<Vector3> waypoints = GenerateWayPoints(berth);
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

		public void OnTransitDeparture(TransitControl.Entry entry)
		{
			_shipsInTraffic.Remove(entry.Ship);
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

			if (!args.WereRemoved)
			{
				AddBerths(args.Berths);
			}
		}

		private void AddBerths(List<ShipBerth> berths)
		{
			var isFirst = !_berths.Any();
			_berths.AddRange(berths);
			if (isFirst && _holder.Count > 0)
			{
				RemoveShipsFromHold();
			}
		}

		private void RemoveShipsFromHold()
		{
			var ships = _holder.ReleaseShips();
			foreach (var ship in ships)
			{
				FindBerthAndAssignToShip(ship);
			}
		}

		public string Name { get { return "TrafficControl"; } }
	}
}