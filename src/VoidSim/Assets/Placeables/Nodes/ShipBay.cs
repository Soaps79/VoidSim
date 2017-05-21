using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    [RequireComponent(typeof(Placeable))]
    public class ShipBay : PlaceableNode
    {
        public int BerthCount;
        private List<ShipBerth> _berths;
        protected static int LastBerthId;

        public override void BroadcastPlacement()
        {
            _berths = gameObject.GetComponentsInChildren<ShipBerth>().ToList();
            foreach (var shipBerth in _berths)
            {
                LastBerthId++;
                shipBerth.name = "ship_berth_" + LastBerthId;
				shipBerth.Initialize();
            }

            MessageHub.Instance.QueueMessage(LogisticsMessages.ShipBerthsUpdated, new ShipBerthsMessageArgs { Berths = _berths });
            BerthCount = _berths.Count;
        }
    }
}