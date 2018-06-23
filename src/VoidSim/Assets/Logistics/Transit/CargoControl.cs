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
        private readonly Dictionary<string, CargoDispatch> _cargoDispatches 
            = new Dictionary<string, CargoDispatch>();
        
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
            if(args == null || !_cargoDispatches.ContainsKey(args.TravelingFrom))
                throw new UnityException("CargoControl got bad args");
            
            _cargoDispatches[args.TravelingFrom].HandleCargoRequested(args.Manifest);
        }
        
        private void HandleLocationRegistered(TransitLocationMessageArgs args)
        {
            if(args == null || args.TransitLocation == null)
                throw new UnityException("CargoControl received bad TransitLocation args");

            var cargoDispatch = args.TransitLocation.GetComponent<CargoDispatch>();
            if (cargoDispatch == null) return;
            
            if(!_cargoDispatches.ContainsKey(args.TransitLocation.ClientName))
                _cargoDispatches.Add(args.TransitLocation.ClientName, cargoDispatch);
        }

        public string Name
        {
            get { return "CargoControl"; }
        }
    }
}