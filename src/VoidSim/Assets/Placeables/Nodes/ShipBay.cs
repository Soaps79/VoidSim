using System.Collections.Generic;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class ShipBayMessageArgs : MessageArgs
    {
        public ShipBay ShipBay;
    }

    [RequireComponent(typeof(Placeable))]
    public class ShipBay : PlaceableNode
    {
        public const string MessageName = "ShipBayPlaced";

        public List<ShipBerth> _berths;

        public override void BroadcastPlacement()
        {
            MessageHub.Instance.QueueMessage(MessageName, new ShipBayMessageArgs { ShipBay = this });
        }
    }
}