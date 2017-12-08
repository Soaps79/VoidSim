using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.Scripts.Serialization;
using Messaging;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	// Mission data is serialized in two parts.
	// The first is the content data, which is static names, descriptions, 
	// goals and such. This is kept with the SerializableObject.
	// The other is for when a game is loaded, and holds the progress that was made when the game was saved.
	[Serializable]
	public class MissionProgressData
	{
		public string Name;
		public List<ProductGoalProgressData> Goals;
	}

	public enum MissionUpdateStatus { Begin, Resume, Complete }

	public class MissionUpdateMessageArgs : MessageArgs
	{
		public MissionUpdateStatus Status;
		public Mission Mission;
	}

	/// <summary>
	/// This object represents a mission instance in progress, and should have whatever hooks are needed for in-game use.
	/// </summary>
	[Serializable]
	public class Mission : ISerializeData<MissionProgressData>
	{
		public const string MessageName = "MissionUpdate";
		public string Name;
		public string DisplayName;
		public string FlavorText;
		public bool IsComplete;
		public List<ProductGoal> Goals;
		public Action<Mission> OnComplete;
		public MissionSO Scriptable;

		public void AddAndActivateGoal(ProductGoal goal)
		{
			if (Goals == null)
				Goals = new List<ProductGoal>();

			ActivateGoal(goal);

			Goals.Add(goal);
			HandleGoalCompleteChange(goal);
		}

		private void ActivateGoal(ProductGoal goal)
		{
			goal.IsActive = true;
			goal.OnCompleteChange += HandleGoalCompleteChange;
		}

		private void HandleGoalCompleteChange(ProductGoal goal)
		{
			if (Goals.All(i => i.IsComplete))
			{
				Goals.ForEach(i => i.IsActive = false);
				IsComplete = true;
				if (OnComplete != null)
					OnComplete(this);
			}
		}

		public MissionProgressData GetData()
		{
			return new MissionProgressData
			{
				Name = Name,
				Goals = Goals.Select(i => i.GetData()).ToList()
			};
		}

		// used when loading progress from saved game
		public void SetProgress(MissionProgressData missionData)
		{
			foreach (var goalInfo in missionData.Goals)
			{
				var goal = Goals.FirstOrDefault(i => i.ProductName == goalInfo.ProductName);
				if(goal == null)
					throw new UnityException("In-progress goal not created from scriptable");

				goal.ElapsedAmount = goalInfo.ElapsedAmount;
				if(goalInfo.IsComplete)
					goal.TriggerComplete(true);
			}
		}
	}
}