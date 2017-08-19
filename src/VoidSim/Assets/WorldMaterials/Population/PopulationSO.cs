using System;
using UnityEngine;

namespace Assets.WorldMaterials.Population
{
	[Serializable]
	public class MoodParams
	{
		[Tooltip("Food consumed per pop")]
		public float FoodPerPop;
		[Tooltip("Water consumed per pop")]
		public float WaterPerPop;
		[Tooltip("Lowest value mood will output")]
		public float MoodMinimum;
	}

	public class PopulationSO : ScriptableObject
	{
		public MoodParams MoodParams;
	}
}