using System.Collections.Generic;
using Assets.Logistics.Ships;
using Assets.Logistics.Transit;
using Assets.Placeables.Nodes;
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
		/// <summary>
		/// Denotes that this item has been deserialized, should continue actions rather than start them
		/// </summary>
	    public bool IsExisting;
    }

    public class TransitRequestedMessageArgs : MessageArgs
    {
	    public bool IsContinuing;
        public string TravelingTo;
        public string TravelingFrom;
        public Ship Ship;
    }

    public class CargoRequestedMessageArgs : MessageArgs
    {
        public string TravelingTo;
        public string TravelingFrom;
        public CargoManifest Manifest;
    }

    public class ShipBerthsMessageArgs : MessageArgs
    {
	    public ShipBay ShipBay;
        public List<ShipBerth> Berths;
    }
}