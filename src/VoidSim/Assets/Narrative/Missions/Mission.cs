using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.Scripts.Serialization;

namespace Assets.Narrative.Missions
{
	[Serializable]
	public class MissionProgressData
	{
		
	}

	[Serializable]
	public class Mission : ISerializeData<MissionProgressData>
	{
		public string Name;
		public string DisplayName;
		public string FlavorText;
		public bool IsComplete;
		public List<ProductGoal> Goals;
		public Action<Mission> OnComplete;

		private void ActivateGoal(ProductGoal goal)
		{
			goal.IsActive = true;
			goal.OnCompleteChange += HandleCompleteChange;
		}

		public void AddAndActivateGoal(ProductGoal goal)
		{
			if (Goals == null)
				Goals = new List<ProductGoal>();

			ActivateGoal(goal);

			Goals.Add(goal);
			HandleCompleteChange(goal);
		}

		private void HandleCompleteChange(ProductGoal goal)
		{
			if (!Goals.All(i => i.IsComplete))
				return;

			Goals.ForEach(i => i.IsActive = false);
			IsComplete = true;
			if (OnComplete != null)
				OnComplete(this);
		}

		public MissionProgressData GetData()
		{
			throw new NotImplementedException();
		}
	}
}