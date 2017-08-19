using System;
using Assets.Scripts;
using Assets.Station;
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
		public int BaseLeisure;
	}

	[Serializable]
	public class EmploymentParams
	{
		public TimeLength EmploymentUpdateTimeLength;
		public int EmploymentUpdateCount;
		public float BaseEmployChance;
	}

	public class PopulationSO : ScriptableObject
	{
		public int InitialCount;
		public int BaseCapacity;
		public MoodParams MoodParams;
		public EmploymentParams EmploymentParams;
	}
}