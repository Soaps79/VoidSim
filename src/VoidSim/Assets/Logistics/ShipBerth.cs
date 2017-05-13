using System;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
    public enum ShipSize
    {
        Corvette, Freighter
    }

    public class ShipBerth : QScript
    {
        public ShipSize ShipSize;
        private TrafficShip _ship;
        public bool IsInUse { get; private set; }

        public Action<TrafficShip> OnShipDock;
        public Action<TrafficShip> OnShipUndock;

        // ship has docked and is ready to be serviced
        public void ConfirmLanding(TrafficShip ship)
        {
            _ship = ship;
            if (OnShipDock != null)
                OnShipDock(_ship);

            Debug.Log("Ship landing confirmed, begin servicing");
        }

        public void CompleteServicing()
        {
            _ship.BeginDeparture();
            IsInUse = false;

            if (OnShipUndock != null)
                OnShipUndock(_ship);

            Debug.Log("Service complete");
        }
    }
}