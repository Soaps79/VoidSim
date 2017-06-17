using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipSOLookup
	{
		// singleton, but no need to be a MonoBehavior, so not using the QGame one
		private static ShipSOLookup _instance = new ShipSOLookup();

		private static List<ShipSO> _ships;
		public static ShipSOLookup Instance { get { return _instance; } }

		private ShipSOLookup()
		{
			Enable();
		}

		static ShipSOLookup()
		{
			Enable();
		}

		private static void Enable()
		{
			_ships = Resources.LoadAll<ShipSO>("Ships").ToList();
		}
		
		public List<ShipSO> GetShips()
		{
			return _ships;
		}
	}
}