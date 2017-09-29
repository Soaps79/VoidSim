using System.Collections.Generic;
using System.Linq;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Handles simple elapsed vs total amounts for product goals
	/// </summary>
	public abstract class ProductGoalTrackerBase
	{
		public abstract GoalType GoalType { get; }

		public List<ProductAmountGoal> Goals = new List<ProductAmountGoal>();
		
		public void AddGoal(ProductAmountGoal goal)
		{
			Goals.Add(goal);
		}

		public void HandleProductupdate(int productId, int amount)
		{
			var completedGoals = new List<ProductAmountGoal>();
			
				var goals = Goals.Where(i => i.ProductId == productId);
				if (!goals.Any())
					return;

			// this is just one way to compare, will need to extract and vary
			foreach (var productGoal in goals)
			{
				productGoal.ElapsedAmount += amount;
				if (productGoal.ElapsedAmount > productGoal.TotalAmount)
					completedGoals.Add(productGoal);
			}

			if (!completedGoals.Any())
				return;

			foreach (var goal in completedGoals)
			{
				goal.TriggerComplete(true);
			}
			Goals.RemoveAll(i => completedGoals.Contains(i));
		}
	}
}