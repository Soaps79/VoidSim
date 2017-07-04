using UnityEngine;

namespace Assets.Logistics.Ships
{
	public class ShipSO : ScriptableObject
	{
		public float MinScale;
		public float MaxScale;
		public float MinTravelTime;
		public float MaxTravelTime;
		public Sprite Sprite;
		// this is currently set by the lookup
		// can move into editor if different ships need different prefabs
		[HideInInspector] public TrafficShip TrafficShipPrefab;

		public float RandomizedTravelTime
		{
			get { return Random.Range(MinTravelTime, MaxTravelTime); }
		}
	}
}