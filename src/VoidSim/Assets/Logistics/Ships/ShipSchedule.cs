using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	[Serializable]
	public class TrafficScheduleEntry
	{
		public float InitialDelay;
		public float RegularDelay;
		public string SOName;
	}

	public class ShipSchedule : ScriptableObject
	{
		public List<TrafficScheduleEntry> Entries;
	}
}