using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Scripts;
using Logistics.Transit;
using Messaging;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;

namespace Assets.Logistics.Transit
{
	public class TransitControl : QScript, IMessageListener
	{
		// Trying a new pattern:
		// Management classes have an embedded public class simply called Entry.
		// Made because this project has a lot of classes that have a list of XEntry, 
		// seeing if there's reusable pieces to come of it
		public class Entry
		{
			public int Id;
			public TransitLocation TravelingTo;
			public TransitLocation TravelingFrom;
			public Ship Ship;
		}

		private readonly Dictionary<string, TransitLocation> _locations = new Dictionary<string, TransitLocation>();
		[SerializeField] private TimeLength _journeyTime;
		private float _journeySeconds;
	    [SerializeField] private List<Ship> _ships = new List<Ship>();

		public List<TransitLocation> GetTransitLocations()
		{
			return _locations.Values.ToList();
		}

		public TransitLocation GetTransitLocation(string locationName)
		{
			return _locations.ContainsKey(locationName) ? _locations[locationName] : null;
		}

		private readonly List<Entry> _entries = new List<Entry>();
		
		public TransitControl()
		{
			OnEveryUpdate += UpdateEntries;
		}

		void Start()
		{
			_journeySeconds = Locator.WorldClock.GetSeconds(_journeyTime);
			Locator.MessageHub.AddListener(this, LogisticsMessages.RegisterLocation);
			Locator.MessageHub.AddListener(this, LogisticsMessages.TransitRequested);
		    Locator.MessageHub.AddListener(this, LogisticsMessages.ShipCreated);
        }

		// move ships towards their destinations, inform the location when a ship arrives
		// it is on the locations to direct the ships from there
		private void UpdateEntries()
		{
			if (!_entries.Any()) return;

		    foreach (var entry in _entries)
			{
				entry.Ship.Ticker.ElapsedTicks += GetDelta();
			}

			var completed = _entries.Where(i => 
				i.Ship.Ticker.ElapsedTicks >= i.Ship.Ticker.TotalTicks).ToList();

			if (!completed.Any())
				return;

			foreach (var entry in completed)
			{
				entry.TravelingTo.HandleTransitArrival(entry.Ship);
			}
			_entries.RemoveAll(i => completed.Contains(i));
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.RegisterLocation && args != null)
				HandleRegisterLocation(args as TransitLocationMessageArgs);

			else if (type == LogisticsMessages.TransitRequested && args != null)
				HandleTransitRequested(args as TransitRequestedMessageArgs);

			else if (type == LogisticsMessages.ShipCreated && args != null)
			    HandleShipCreated(args as ShipCreatedMessageArgs);

        }

	    private void HandleShipCreated(ShipCreatedMessageArgs args)
	    {
	        if(args == null || args.Ship == null)
                throw new UnityException("TransitControl given bad ship args");

            _ships.Add(args.Ship);
	    }

	    // handle bad data, add location
		private void HandleRegisterLocation(TransitLocationMessageArgs args)
		{
			if(args == null || args.TransitLocation == null)
				throw new UnityException("TransitControl given bad location register message data");

			if(_locations.ContainsKey(args.TransitLocation.ClientName))
				throw new UnityException("Transit location registered more than once");

			_locations.Add(args.TransitLocation.ClientName, args.TransitLocation);
			Debug.Log(string.Format("Location registered: {0}", args.TransitLocation.ClientName));
		}

		// handle bad data, discover travel time and begin tracking
		private void HandleTransitRequested(TransitRequestedMessageArgs args)
		{
			if (string.IsNullOrEmpty(args?.TravelingTo) 
                || string.IsNullOrEmpty(args.TravelingFrom) 
                || args.Ship == null 
                || !_locations.ContainsKey(args.TravelingTo) 
                || !_locations.ContainsKey(args.TravelingFrom))
				throw new UnityException("TransitControl given bad transit request message data");

			var source = _locations[args.TravelingFrom];
			var destination = _locations[args.TravelingTo];
			if(!args.IsContinuing)
				args.Ship.Ticker.Reset(CalculateTravelTime(source, destination));

			var entry = new Entry
			{
				Id = Locator.LastId.GetNext("transit_entry"),
				TravelingFrom = source,
				TravelingTo = destination,
				Ship = args.Ship
			};
			_entries.Add(entry);
			source.HandleTransitDeparture(entry.Ship);
		}

		private float CalculateTravelTime(TransitLocation from, TransitLocation to)
		{
			return _journeySeconds;
		}

		public string Name { get { return name; } }
	}
}