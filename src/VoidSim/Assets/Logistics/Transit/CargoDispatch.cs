using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Logistics.Transit;
using QGame;
using UnityEngine;

namespace Assets.Logistics.Transit
{
	[RequireComponent(typeof(TransitLocation))]
	public class CargoDispatch : QScript
	{
		private readonly List<CargoManifest> _awaitingDispatch = new List<CargoManifest>();
		private List<Ship> _ships;
		private TransitLocation _transitLocation;

		private void Start()
		{
			_transitLocation = GetComponent<TransitLocation>();
		}

		public static Ship FindCarrier(List<Ship> ships, CargoManifest manifest)
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

		private static Ship FindShipHeadingTo(List<Ship> ships, CargoManifest manifest)
		{
			var valid = ships.Where(i => i.Navigation.CurrentDestination == manifest.Shipper).ToList();
			return valid.Any() ? valid[Random.Range(0, valid.Count - 1)] : null;
		}
	}
}