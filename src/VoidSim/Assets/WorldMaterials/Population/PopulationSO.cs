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
		public float MaxLeisureBonus;
	}

	[Serializable]
	public class EmploymentParams
	{
		public TimeLength EmploymentUpdateTimeLength;
		public int EmploymentUpdateCount;
		public float BaseEmployChance;
	}

    [Serializable]
    public class GenerationParams
    {
        public int MinNamesLoaded;
        public int MaxNamesLoaded;
    }

	public class PopulationSO : ScriptableObject
	{
		public int InitialCount;
		public int BaseCapacity;
		public GameColors Colors;
		public MoodParams MoodParams;
		public EmploymentParams EmploymentParams;
	    public GenerationParams GenerationParams;
	}
}