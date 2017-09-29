using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using UnityEngine;

namespace Assets.Narrative.Missions
{
	[Serializable]
	public class Mission
	{
		public string Name;
		public string FlavorText;
		public List<ProductAmountGoal> Goals;

		public void Initialize()
		{
			foreach (var goal in Goals)
			{
				goal.IsActive = true;
				goal.OnCompleteChange += HandleCompleteChange;
			}
		}

		private void HandleCompleteChange(ProductAmountGoal goal)
		{
			if (!Goals.All(i => i.IsComplete))
				return;

			Goals.ForEach(i => i.IsActive = false);
		}
	}
}