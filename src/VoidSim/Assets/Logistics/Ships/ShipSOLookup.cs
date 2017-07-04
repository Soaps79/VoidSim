using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipSOLookup : ScriptableObject
	{
		private static List<ShipSO> _ships;
		public TrafficShip TrafficShipPrefab;
		
		void OnEnable()
		{
			_ships = Resources.LoadAll<ShipSO>("Ships").ToList();
			_ships.ForEach(i => i.TrafficShipPrefab = TrafficShipPrefab);
		}
		
		public List<ShipSO> GetShips()
		{
			return _ships;
		}
	}
}