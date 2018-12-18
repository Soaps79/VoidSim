using System.Collections.Generic;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics.Transit
{
    /// <summary>
    /// Recieves cargo requests, distributes to TravelingFrom Locations
    /// </summary>
    public class CargoControl : QScript, IMessageListener
    {
        private readonly Dictionary<string, CargoDispatch> _dispatches = new Dictionary<string, CargoDispatch>();

        private void Start()
        {
            Locator.MessageHub.AddListener(this, LogisticsMessages.RegisterLocation);
            Locator.MessageHub.AddListener(this, LogisticsMessages.CargoTransitRequested);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.RegisterLocation && args != null)
            {
                HandleRegisterLocation(args as TransitLocationMessageArgs);
            }
            if (type == LogisticsMessages.CargoTransitRequested && args != null)
            {
                HandleCargoRequested(args as CargoTransitRequestedMessageArgs);
            }
        }

        private void HandleCargoRequested(CargoTransitRequestedMessageArgs args)
        {
            if (args == null || args.Manifest == null || !_dispatches.ContainsKey(args.Manifest.Shipper))
                throw new UnityException("CargoControl given bad cargo transit requested args");

            _dispatches[args.Manifest.Shipper].HandleManifest(args.Manifest);
        }

        private void HandleRegisterLocation(TransitLocationMessageArgs args)
        {
            if(string.IsNullOrEmpty(args?.TransitLocation.ClientName))
                throw new UnityException("CangoControl given bad Location Registered args");

            var dispatch = args.TransitLocation.GetComponent<CargoDispatch>();
            if(dispatch != null)
                _dispatches.Add(args.TransitLocation.ClientName, dispatch);
        }

        public string Name => "CargoControl";
    }
}