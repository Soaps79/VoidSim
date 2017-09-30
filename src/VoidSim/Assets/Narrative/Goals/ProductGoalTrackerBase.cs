using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Narrative.Goals
{
	public interface IGoalTracker
	{
		GoalType GoalType { get; }
		string DisplayString { get; }
		void AddGoal(ProductGoal goal);
		void Prune();
	}

	/// <summary>
	/// Handles simple elapsed vs total amounts for product goals
	/// </summary>
	[Serializable]
	public abstract class ProductGoalTrackerBase : IGoalTracker
	{
		public abstract GoalType GoalType { get; }

		protected virtual void OnGoalAdded(ProductGoal goal) { }

		public List<ProductGoal> Goals = new List<ProductGoal>();
		
		public void AddGoal(ProductGoal goal)
		{
			Goals.Add(goal);
			OnGoalAdded(goal);
		}

		public void HandleProductupdate(int productId, int amount)
		{
			var goals = Goals.Where(i => i.ProductId == productId);
			if (!goals.Any())
				return;

			var completedGoals = new List<ProductGoal>();

			// this is just one way to compare, will need to extract and vary
			foreach (var productGoal in goals)
			{
				productGoal.ElapsedAmount += amount;
				if (productGoal.ElapsedAmount >= productGoal.TotalAmount)
					completedGoals.Add(productGoal);
			}

			if (!completedGoals.Any())
				return;

			// Missions will handle their completed goals, setting IsActive to false if they're done being tracked
			foreach (var goal in completedGoals)
			{
				goal.TriggerComplete(true);
			}
			Prune();
		}

		public void Prune()
		{
			Goals.RemoveAll(i => !i.IsActive);
		}

		public string DisplayString
		{
			get
			{
				// rafactor to actually use the builder
				var builder = new StringBuilder();
				builder.Append(Goals.Aggregate("",
					(current, goal) => current + "  " + goal.ProductName + " " + goal.ElapsedAmount + "/" + goal.TotalAmount));

				return builder.ToString();
			}
		}
	}
}