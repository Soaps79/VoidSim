using System.Collections.Generic;
using System.Linq;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
    public interface ITransitLocation
    {
        string ClientName { get; }
        void OnTransitArrival(TransitRegister.Entry entry);
        void OnTransitDeparture(TransitRegister.Entry entry);
    }

    public class EmptyTransitLocation : ITransitLocation
    {
        public string ClientName { get { return "EmptyTransitLocation"; } }
        public void OnTransitArrival(TransitRegister.Entry entry) { }
        public void OnTransitDeparture(TransitRegister.Entry entry) { }
    }

    public class TransitRegister : QScript, IMessageListener
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
            public float TotalTravelTime;
            public float ElapsedTravelTime;

            public float TimeRemainingAsZeroToOne
            {
                get { return TotalTravelTime > 0 ? ElapsedTravelTime / TotalTravelTime : 0; }
            }
        }

        private readonly Dictionary<string, ITransitLocation> _locations = new Dictionary<string, ITransitLocation>();
        public List<ITransitLocation> GetTransitLocations()
        {
            return _locations.Values.ToList();
        }

        private readonly List<Entry> _entries = new List<Entry>();
        private int _lastId;

        public TransitRegister()
        {
            OnEveryUpdate += UpdateEntries;
        }

        void Start()
        {
            KeyValueDisplay.Instance.Add("Transit", () => GenerateDisplayText);
            MessageHub.Instance.AddListener(this, LogisticsMessages.RegisterLocation);
            MessageHub.Instance.AddListener(this, LogisticsMessages.TransitRequested);
        }

        private object GenerateDisplayText
        {
            get
            {
                return _entries.Any()
                    ? _entries.Aggregate(string.Format("{0}, ", _entries.Count), (name, entry)
                        => string.Format("{0:G3} ", entry.TimeRemainingAsZeroToOne)) : "";
            }
        }

        // move ships towards their destinations, inform the location when a ship arrives
        // it is on the locations to direct the ships from there
        private void UpdateEntries(float delta)
        {
            if (!_entries.Any()) return;

            foreach (var entry in _entries)
            {
                entry.ElapsedTravelTime += delta;
            }

            var completed = _entries.Where(i => i.ElapsedTravelTime >= i.TotalTravelTime);
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
                throw new UnityException("TransitRegister given bad location register message data");

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
                throw new UnityException("TransitRegister given bad transit request message data");

            _lastId++;
            var source = _locations[args.TravelingFrom];
            var destination = _locations[args.TravelingTo];
            var travelTime = CalculateTravelTime(source, destination);
            var entry = new Entry
            {
                Id = _lastId,
                TotalTravelTime = travelTime,
                TravelingFrom = source,
                TravelingTo = destination,
                Ship = args.Ship
            };
            _entries.Add(entry);
            source.OnTransitDeparture(entry);
            //Debug.Log(string.Format("Transit requested: {0} {1} to {2}", args.Ship.Name, args.TravelingFrom, args.TravelingTo));
        }

        private float CalculateTravelTime(ITransitLocation from, ITransitLocation to)
        {
            return 5;
        }

        public string Name { get { return name; } }
    }
}