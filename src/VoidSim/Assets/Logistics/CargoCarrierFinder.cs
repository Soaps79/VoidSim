using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using UnityEngine;

namespace Assets.Logistics
{
	public static class CargoCarrierFinder
	{
		public static Ship FindCarrier(List<Ship> ships, TradeManifest manifest)
		{
			var ship = FindShipHeadingTo(ships, manifest);
			//if (ship == null)
			//	ship = FindRandomShipWithRoom(ships);

			return ship;
		}

		private static Ship FindRandomShipWithRoom(List<Ship> ships)
		{
			var rand = Random.Range(0, ships.Count - 1);
			return ships[rand];
		}

		private static Ship FindShipHeadingTo(List<Ship> ships, TradeManifest manifest)
		{
			var valid = ships.Where(i => i.Navigation.CurrentDestination.ClientName == manifest.Seller).ToList();
			return valid.Any() ? valid[Random.Range(0, valid.Count - 1)] : null;
		}
	}
}