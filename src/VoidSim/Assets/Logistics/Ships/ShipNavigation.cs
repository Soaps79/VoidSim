using System.Collections.Generic;
using Messaging;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipNavigation
	{
		public ITransitLocation LastDeparted { get; private set; }
		public ITransitLocation CurrentDestination { get; private set; }

		public Ship ParentShip;

		private readonly Queue<ITransitLocation> _locations = new Queue<ITransitLocation>();

		// initializes with empty location objects
		public ShipNavigation()
		{
			LastDeparted = new EmptyTransitLocation();
			CurrentDestination = new EmptyTransitLocation();
		}

		public void BeginTrip()
		{
			MessageHub.Instance.QueueMessage(LogisticsMessages.TransitRequested, new TransitRequestedMessageArgs
			{
				Ship = ParentShip,
				TravelingFrom = LastDeparted.ClientName,
				TravelingTo = CurrentDestination.ClientName
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

		public void AddLocation(ITransitLocation location)
		{
			if (location == null)
				throw new UnityException("Ship navigation given bad location data");

			_locations.Enqueue(location);
		}
	}
}