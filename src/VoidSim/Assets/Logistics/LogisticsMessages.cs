using System.Collections.Generic;
using Messaging;

namespace Assets.Logistics
{
    public static class LogisticsMessages
    {
        public const string RegisterLocation = "RegisterTransitLocation";
        public const string ShipCreated = "ShipCreated";
        public const string TransitRequested = "TransitRequested";
        public const string CargoRequested = "CargoRequested";
        public const string ShipBerthsUpdated = "ShipBerthsUpdated";
    }

    public class TransitLocationMessageArgs : MessageArgs
    {
        public ITransitLocation TransitLocation;
    }

    public class ShipCreatedMessageArgs : MessageArgs
    {
        public Ship Ship;
    }

    public class TransitRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public string TravelingFrom;
        public Ship Ship;
    }

    public class CargoRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public string TravelingFrom;
        public TradeManifest Manifest;
    }

    public class ShipBerthsMessageArgs : MessageArgs
    {
        public bool WereRemoved;
        public List<ShipBerth> Berths;
    }
}