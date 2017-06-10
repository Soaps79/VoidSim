using System.Collections.Generic;
using System.Linq;
using Messaging;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipNavigationData
	{
		public List<string> Locations;
		public string LastDeparted;
		public string CurrentDestination;
	}

	public class ShipNavigation
	{
		public string LastDeparted { get; private set; }
		public string CurrentDestination { get; private set; }

		public Ship ParentShip;

		private readonly Queue<string> _locations = new Queue<string>();

		// initializes with empty location objects
		public ShipNavigation()
		{
			LastDeparted = "";
			CurrentDestination = "";
		}

		public ShipNavigation(ShipNavigationData data)
		{
			LoadFromData(data);
		}

		public void BeginTrip(bool isContinuing = false)
		{
			MessageHub.Instance.QueueMessage(LogisticsMessages.TransitRequested, new TransitRequestedMessageArgs
			{
				IsContinuing = isContinuing,
				Ship = ParentShip,
				TravelingFrom = LastDeparted,
				TravelingTo = CurrentDestination
			});
		}

		public void CompleteDestination()
		{
			// will currently cycle through all known destination
			CycleLocations();
			BeginTrip();
		}

		public void CycleLocations()
		{
			LastDeparted = _locations.Dequeue();
			_locations.Enqueue(LastDeparted);
			CurrentDestination = _locations.Peek();
		}

		public void AddLocation(string location)
		{
			if (location == null)
				throw new UnityException("Ship navigation given bad location data");

			_locations.Enqueue(location);
		}

		private void LoadFromData(ShipNavigationData data)
		{
			foreach (var location in data.Locations)
			{
				_locations.Enqueue(location);
			}
			CurrentDestination = data.CurrentDestination;
			LastDeparted = data.LastDeparted;
		}

		public ShipNavigationData GetData()
		{
			return new ShipNavigationData
			{
				CurrentDestination = CurrentDestination,
				LastDeparted = LastDeparted,
				Locations = _locations.ToList()
			};
		}
	}
}