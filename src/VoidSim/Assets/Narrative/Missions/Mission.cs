using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;

namespace Assets.Narrative.Missions
{
	[Serializable]
	public class Mission
	{
		public string Name;
		public string FlavorText;
		public bool IsComplete;
		public List<ProductAmountGoal> Goals;
		public Action<Mission> OnComplete;

		public void Initialize()
		{
			foreach (var goal in Goals)
			{
				ActivateGoal(goal);
			}
		}

		private void ActivateGoal(ProductAmountGoal goal)
		{
			goal.IsActive = true;
			goal.OnCompleteChange += HandleCompleteChange;
		}

		public void AddAndActivateGoal(ProductAmountGoal goal)
		{
			if (Goals == null)
				Goals = new List<ProductAmountGoal>();

			ActivateGoal(goal);

			Goals.Add(goal);
			HandleCompleteChange(goal);
		}

		private void HandleCompleteChange(ProductAmountGoal goal)
		{
			if (!Goals.All(i => i.IsComplete))
				return;

			Goals.ForEach(i => i.IsActive = false);
			IsComplete = true;
			if (OnComplete != null)
				OnComplete(this);
		}
	}
}