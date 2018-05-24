using System.Collections.Generic;
using Assets.Logistics;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;

namespace Logistics.Transit
{
    public class CargoControl : QScript, IMessageListener
    {
        private readonly Dictionary<string, TransitLocation> _locations 
            = new Dictionary<string, TransitLocation>();
        
        private void Start()
        {
            Locator.MessageHub.AddListener(this, LogisticsMessages.RegisterLocation);
            Locator.MessageHub.AddListener(this, LogisticsMessages.CargoRequested);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.CargoRequested && args != null)
                HandleCargoRequested(args as CargoRequestedMessageArgs);
                
            if (type == LogisticsMessages.RegisterLocation && args != null)
                HandleLocationRegistered(args as TransitLocationMessageArgs);
        }

        private void HandleCargoRequested(CargoRequestedMessageArgs args)
        {
            if(args == null || !_locations.ContainsKey(args.TravelingFrom))
                throw new UnityException("CargoControl got bad args");
            
            _locations[args.TravelingFrom].HandleCargoRequested(args.Manifest);
        }
        
        private void HandleLocationRegistered(TransitLocationMessageArgs args)
        {
            if(args == null || args.TransitLocation == null)
                throw new UnityException("CargoControl received bad TransitLocation args");
            
            if(!_locations.ContainsKey(args.TransitLocation.ClientName))
                _locations.Add(args.TransitLocation.ClientName, args.TransitLocation);
        }

        public string Name
        {
            get { return "CargoControl"; }
        }
    }
}