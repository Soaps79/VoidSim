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

        // might not be needed
        public bool TryAcceptShip(Ship ship)
        {
            if (IsInUse || ship.Size != ShipSize)
                return false;

            if (!ship.AcknowledgeBerth(this))
            {
                Debug.Log("Ship refused to acknowledge Berth");
                return false;
            }

            IsInUse = true;
            return true;
        }

        // ship has docked and is ready to be serviced
        public void ConfirmLanding(TrafficShip ship)
        {
            _ship = ship;
            Debug.Log("Ship landing confirmed, begin servicing");
            var node = StopWatch.AddNode("ShipService", 5, true);
            node.OnTick += CompleteServicing;
        }

        private void CompleteServicing()
        {
            Debug.Log("Service complete");
            _ship.BeginDeparture();
        }
    }
}