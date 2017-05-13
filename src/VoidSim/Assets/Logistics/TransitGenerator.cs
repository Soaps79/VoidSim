﻿using System.Linq;
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
        private Ship _ship;
        private int _lastId;
        private TransitRegister _transitRegister;
        [SerializeField] private GameObject _cargoShip;

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
            _ship = new Ship();
            var locations = _transitRegister.GetTransitLocations();
            if(locations.Count < 2) throw new UnityException("TransitGenerator found less than 2 locations");

            var navigation = new ShipNavigation { ParentShip = _ship };
            locations.ForEach(i => navigation.AddLocation(i));
            _ship.Initialize(navigation, _cargoShip);

            navigation.CompleteDestination();
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.CargoRequested && args != null)
                HandleTransitRequest(args as CargoRequestedMessageArgs);
        }

        private void HandleTransitRequest(CargoRequestedMessageArgs args)
        {
            if(args == null) throw new UnityException("TransitGenerator recieved bad transit request args");
            _lastId++;
            args.Manifest.Id = _lastId;
            _ship.AddManifest(args.Manifest);
        }

        public string Name { get { return name; } }
    }
}