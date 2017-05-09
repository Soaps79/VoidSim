using System.Collections.Generic;
using Messaging;

namespace Assets.Logistics
{
    public static class LogisticsMessages
    {
        public const string RegisterLocation = "RegisterTransitLocation";
        public const string TransitRequested = "TransitRequested";
        public const string CargoRequested = "CargoRequested";
        public const string ShipBerthsUpdated = "ShipBerthsPlaced";
    }

    public class CargoRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public string TravelingFrom;
        public TradeManifest Manifest;
    }

    public class TransitRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public string TravelingFrom;
        public Ship Ship;
    }

    public class TransitLocationMessageArgs : MessageArgs
    {
        public ITransitLocation TransitLocation;
    }

    public class ShipBerthsMessageArgs : MessageArgs
    {
        public bool WereRemoved;
        public List<ShipBerth> Berths;
    }
}