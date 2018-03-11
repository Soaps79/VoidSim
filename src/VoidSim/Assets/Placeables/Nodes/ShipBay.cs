using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	[RequireComponent(typeof(Placeable))]
	public class ShipBay : PlaceableNode<ShipBay>
	{
		protected override ShipBay GetThis() { return this; }
		public override string NodeName { get { return "ShipBay"; } }
		public int BerthCount;
		public List<ShipBerth> Berths { get; private set; }

		public override void Initialize(PlaceableData data)
		{
			Berths = gameObject.GetComponentsInChildren<ShipBerth>().ToList();
			foreach (var shipBerth in Berths)
			{
				shipBerth.name = string.Format("{0}_{1}", name, shipBerth.Index);
				shipBerth.Initialize();
			}
			BerthCount = Berths.Count;
		}

		public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(LogisticsMessages.ShipBerthsUpdated, new ShipBerthsMessageArgs
			{
				ShipBay = this,
				Berths = Berths
			});
		}
	}
}