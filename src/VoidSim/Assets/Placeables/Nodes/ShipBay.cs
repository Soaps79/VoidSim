using System.Collections.Generic;
using System.Linq;
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

        public int BerthCount;
        private List<ShipBerth> _berths;

        public override void BroadcastPlacement()
        {
            MessageHub.Instance.QueueMessage(MessageName, new ShipBayMessageArgs { ShipBay = this });
            _berths = gameObject.GetComponentsInChildren<ShipBerth>().ToList();
            BerthCount = _berths.Count;
        }
    }
}