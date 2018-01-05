using System;
using System.Collections.Generic;
using Assets.Scripts;
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
    public class PersonNeedsInfo
    {
        public PersonNeedsType Type;
        public string DisplayName;
		[Tooltip("Newly generated pop will have value between MinInitial and MaxInitial")]
        public float MinInitialValue;
		[Tooltip("Newly generated pop will have value between MinInitial and MaxInitial")]
        public float MaxInitialValue;
		[Tooltip("Value will never go below this amount")]
        public float MinValue;
		[Tooltip("Value will never go above this amount")]
        public float MaxValue;
		[Tooltip("Person will start looking to fulfill when value goes below this")]
        public float MinTolerance;
        [Tooltip("Person will consider returning to work when value goes above this")]
        public float MinFulfillment;
        [Tooltip("Chance a person will move when unfulfilled. Increases to 1.0 as value reaches 0.")]
        public float StartingWantToMove;
    }

    [Serializable]
    public class GenerationParams
    {
        [Tooltip("List of available names will refill when down to this amount")]
        public int MinNamesLoaded;
        [Tooltip("List filled up to this amount")]
        public int MaxNamesLoaded;
        [Tooltip("Template needs for all Persons")]
        public List<PersonNeedsInfo> ResidentNeeds;
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