using System.Collections.Generic;
using System.Linq;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
    public class TrafficControl : QScript, IMessageListener, ITransitLocation
    {
        public string ClientName { get { return "Station"; } }

        private List<ShipBerth> _berths = new List<ShipBerth>();
        // do something with this
        private readonly Queue<Ship> _queuedShips = new Queue<Ship>();
        private readonly List<Ship> _shipsInTraffic = new List<Ship>();

        void Start()
        {
            MessageHub.Instance.AddListener(this, LogisticsMessages.ShipBerthsUpdated);
            MessageHub.Instance.QueueMessage(LogisticsMessages.RegisterLocation, 
                new TransitLocationMessageArgs { TransitLocation = this });
        }

        public void OnTransitArrival(TransitRegister.Entry entry)
        {
            Debug.Log("Ship arrived to airspace");

            if (!_berths.Any())
            {
                // this is temp, shouldn't be a thing eventually
                Debug.Log("Ship arrived before berths placed");
                entry.Ship.CompleteVisit();
            }

            var berth = _berths.FirstOrDefault(i => i.ShipSize == entry.Ship.Size);
            if (berth == null)
            {
                _queuedShips.Enqueue(entry.Ship);
                return;
            }

            entry.Ship.AcknowledgeBerth(berth);
            _shipsInTraffic.Add(entry.Ship);
        }

        public void OnTransitDeparture(TransitRegister.Entry entry)
        {
            _shipsInTraffic.Remove(entry.Ship);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.ShipBerthsUpdated && args != null)
                HandleBerthsUpdate(args as ShipBerthsMessageArgs);
        }

        private void HandleBerthsUpdate(ShipBerthsMessageArgs args)
        {
            if(args == null || !args.Berths.Any())
                throw new UnityException("TrafficControl given bad berths message");

            if(!args.WereRemoved)
                _berths.AddRange(args.Berths);
        }

        public string Name { get { return "TrafficControl"; } }
    }
}