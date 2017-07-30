using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Scripts;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	[RequireComponent(typeof(Placeable))]
	public class ShipBay : PlaceableNode
	{
		public override string NodeName { get { return "ShipBay"; } }
		public int BerthCount;
		private List<ShipBerth> _berths;

		public override void BroadcastPlacement()
		{
			if (name == DefaultName)
			{
				var lastId = Locator.LastId.GetNext("ship_bay");
				name = "ship_bay_" + lastId;
			}

			_berths = gameObject.GetComponentsInChildren<ShipBerth>().ToList();
			foreach (var shipBerth in _berths)
			{
				shipBerth.name = string.Format("{0}_{1}", name, shipBerth.Index);
				shipBerth.Initialize();
			}

			MessageHub.Instance.QueueMessage(LogisticsMessages.ShipBerthsUpdated, new ShipBerthsMessageArgs { Berths = _berths });
			BerthCount = _berths.Count;
		}
	}
}