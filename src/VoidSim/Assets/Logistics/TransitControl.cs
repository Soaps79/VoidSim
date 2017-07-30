using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;
using TimeLength = Assets.Scripts.TimeLength;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Logistics
{
	public interface ITransitLocation
	{
		string ClientName { get; }
		void OnTransitArrival(TransitControl.Entry entry);
		void OnTransitDeparture(TransitControl.Entry entry);
		void Resume(Ship ship);
		bool IsSimpleHold { get; }
	}

	public class EmptyTransitLocation : ITransitLocation
	{
		public string ClientName { get { return "EmptyTransitLocation"; } }
		public void OnTransitArrival(TransitControl.Entry entry) { }
		public void OnTransitDeparture(TransitControl.Entry entry) { }
		public void Resume(Ship ship) { }
		public bool IsSimpleHold { get { return true; } }
	}

	public class TransitControl : QScript, IMessageListener
	{
		/// <summary>
		/// Trying a new pattern:
		/// Management classes have an embedded public class simply called Entry.
		/// Made because this project has a lot of classes that have a list of XEntry, 
		/// seeing if there's reusable pieces to come of it
		/// </summary>
		public class Entry
		{
			public int Id;
			public ITransitLocation TravelingTo;
			public ITransitLocation TravelingFrom;
			public Ship Ship;
		}

		private readonly Dictionary<string, ITransitLocation> _locations = new Dictionary<string, ITransitLocation>();
		[SerializeField] private TimeLength _journeyTime;
		private float _journeySeconds;

		public List<ITransitLocation> GetTransitLocations()
		{
			return _locations.Values.ToList();
		}

		public ITransitLocation GetTransitLocation(string locationName)
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
			_journeySeconds = WorldClock.Instance.GetSeconds(_journeyTime);
			MessageHub.Instance.AddListener(this, LogisticsMessages.RegisterLocation);
			MessageHub.Instance.AddListener(this, LogisticsMessages.TransitRequested);
		}

		// move ships towards their destinations, inform the location when a ship arrives
		// it is on the locations to direct the ships from there
		private void UpdateEntries(float delta)
		{
			if (!_entries.Any()) return;

			foreach (var entry in _entries)
			{
				entry.Ship.Ticker.ElapsedTicks += delta;
			}

			var completed = _entries.Where(i => 
				i.Ship.Ticker.ElapsedTicks >= i.Ship.Ticker.TotalTicks).ToList();

			if (!completed.Any())
				return;

			foreach (var entry in completed)
			{
				entry.TravelingTo.OnTransitArrival(entry);
			}
			_entries.RemoveAll(i => completed.Contains(i));
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == LogisticsMessages.RegisterLocation && args != null)
				HandleRegisterLocation(args as TransitLocationMessageArgs);

			else if (type == LogisticsMessages.TransitRequested && args != null)
				HandleTransitRequested(args as TransitRequestedMessageArgs);
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
			if (args == null
				|| string.IsNullOrEmpty(args.TravelingTo)
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
			source.OnTransitDeparture(entry);
		}

		private float CalculateTravelTime(ITransitLocation from, ITransitLocation to)
		{
			return _journeySeconds;
		}

		public string Name { get { return name; } }
	}
}