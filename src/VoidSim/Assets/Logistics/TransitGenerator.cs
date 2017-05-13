using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
    /// <summary>
    /// Handles creation of ships, initializing their navigation, and turning accepted trades into
    /// manifests for the ships to track
    /// </summary>
    [RequireComponent(typeof(TransitRegister))]
    public class TransitGenerator : QScript, IMessageListener
    {
        private readonly List<Ship> _ships = new List<Ship>();
        private int _lastManifestId;
        private int _lastShipId;
        private TransitRegister _transitRegister;
        [SerializeField] private GameObject _cargoShip;
        [SerializeField] private ShipSchedule _schedule;

        void Start()
        {
            MessageHub.Instance.AddListener(this, LogisticsMessages.CargoRequested);
            _transitRegister = gameObject.GetComponent<TransitRegister>();
            
            // give locations some time to register
            //  will eventually have an array of ships launching at different times
            var node = StopWatch.AddNode("first_ship", 1, true);
            node.OnTick += InitializeShips;
        }

        private void InitializeShips()
        {
            var locations = _transitRegister.GetTransitLocations();
            if(locations.Count < 2) throw new UnityException("TransitGenerator found less than 2 locations");

            if (_schedule == null)
            {
                Debug.Log("TransitGenerator has no ship schedule");
                return;
            }

            foreach (var entry in _schedule.Entries)
            {
                _lastShipId++;
                var ship = new Ship{ Name = "ship_" + _lastShipId };
                var navigation = new ShipNavigation { ParentShip = ship };
                locations.ForEach(i => navigation.AddLocation(i));
                ship.Initialize(navigation, _cargoShip);
                var node = StopWatch.AddNode(ship.Name, entry.InitialDelay, true);
                node.OnTick += () => navigation.CompleteDestination();
            }
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.CargoRequested && args != null)
                HandleTransitRequest(args as CargoRequestedMessageArgs);
        }

        private void HandleTransitRequest(CargoRequestedMessageArgs args)
        {
            if(args == null) throw new UnityException("TransitGenerator recieved bad cargo request args");
            _lastManifestId++;
            args.Manifest.Id = _lastManifestId;
            var ship = _ships.FirstOrDefault(i => i.Navigation.CurrentDestination.ClientName == "Void") ?? _ships.First();
            ship.AddManifest(args.Manifest);
        }

        public string Name { get { return name; } }
    }
}