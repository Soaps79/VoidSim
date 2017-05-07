using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Void
{
    public static class TransitMessages
    {
        public const string RegisterLocation = "RegisterTransitLocation";
        public const string TransitRequested = "TransitRequested";
    }

    public class Ship
    {
        public ShipType Type;
        public List<ProductAmount> ProductCargo;
    }

    public class TransitRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public ITransitLocation TravelingFrom;
        public Ship Ship;
    }

    public class TransitLocationMessageArgs : MessageArgs
    {
        public ITransitLocation TransitLocation;
    }

    public interface ITransitLocation
    {
        string ClientName { get; }

        void OnTransitComplete(TransitRegister.Entry entry);
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
        private readonly List<Entry> _entries = new List<Entry>();
        private int _lastId;

        public TransitRegister()
        {
            OnEveryUpdate += UpdateEntries;
        }

        void Start()
        {
            KeyValueDisplay.Instance.Add("Transit", () => GenerateDisplayText);
            MessageHub.Instance.AddListener(this, TransitMessages.TransitRequested);
            MessageHub.Instance.AddListener(this, TransitMessages.RegisterLocation);
        }

        private object GenerateDisplayText
        {
            get
            {
                return _entries.Any()
                    ? _entries.Aggregate(string.Format("{0}, ", _entries.Count), (name, entry)
                        => entry.TimeRemainingAsZeroToOne.ToString() + " ") : "";
            }
        }

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
                entry.TravelingFrom.OnTransitComplete(entry);
                entry.TravelingTo.OnTransitComplete(entry);
            }
            _entries.RemoveAll(i => completed.Contains(i));
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == TransitMessages.TransitRequested && args != null)
                HandleTransitRequested(args as TransitRequestedMessageArgs);

            else if (type == TransitMessages.RegisterLocation && args != null)
                HandleRegisterLocation(args as TransitLocationMessageArgs);
        }

        // handle bad data, add location
        private void HandleRegisterLocation(TransitLocationMessageArgs args)
        {
            if(args == null || args.TransitLocation == null)
                throw new UnityException("TransitRegister given bad location register message data");

            if(_locations.ContainsKey(args.TransitLocation.ClientName))
                throw new UnityException("Transit location registered more than once");

            _locations.Add(args.TransitLocation.ClientName, args.TransitLocation);
        }

        // handle bad data, discover travel time and begin tracking
        private void HandleTransitRequested(TransitRequestedMessageArgs args)
        {
            if (args == null 
                || args.TravelingFrom == null
                || string.IsNullOrEmpty(args.TravelingTo)
                || args.Ship == null
                || !_locations.ContainsKey(args.TravelingTo))
                throw new UnityException("TransitRegister given bad transit request message data");

            _lastId++;
            var destination = _locations[args.TravelingTo];
            var travelTime = CalculateTravelTime(args.TravelingFrom, destination);
            _entries.Add(new Entry
            {
                Id = _lastId,
                TotalTravelTime = travelTime,
                TravelingFrom = args.TravelingFrom,
                TravelingTo = destination,
                Ship = args.Ship
            });
        }

        private float CalculateTravelTime(ITransitLocation from, ITransitLocation to)
        {
            return 10;
        }

        public string Name { get; private set; }
    }
}