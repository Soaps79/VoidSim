using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipSOLookup : ScriptableObject
	{
		private static List<ShipSO> _ships;
		
		void OnEnable()
		{
			_ships = Resources.LoadAll<ShipSO>("Ships").ToList();
		}
		
		public List<ShipSO> GetShips()
		{
			return _ships;
		}
	}
}