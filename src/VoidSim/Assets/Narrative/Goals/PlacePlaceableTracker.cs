using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Placeables;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Watches the PlaceableMonitor changes in state (placed, soon removed)
	/// and handles placement goals accodingly
	/// 
	/// It's not pretty
	/// Currently works by toggling a goal's NeedsPlacement flag
	/// It might only work once?
	/// </summary>
	public class PlacePlaceableTracker : IGoalTracker
	{
		public GoalType GoalType { get { return GoalType.PlacePlaceable; } }

		private readonly List<ProductGoal> _goals = new List<ProductGoal>();

		public PlacePlaceableTracker()
		{
			// here be brittle
			var placeablesMonitor = GameObject.Find("core").GetComponent<PlaceablesMonitor>();
			placeablesMonitor.OnPlaced += HandlePlaced;
		}

		private void HandlePlaced(Placeable placeable)
		{
			foreach (var goal in _goals)
			{
				if (goal.ProductName == placeable.PlaceableName)
				{
					if (goal.TotalAmount >= goal.ElapsedAmount)
						goal.ElapsedAmount++;
					goal.NeedsPlacement = false;
					goal.TriggerComplete(true);
				}
			}

			_goals.RemoveAll(i => !i.IsActive);
		}

		public void Prune()
		{
			_goals.RemoveAll(i => !i.IsActive);
		}

		public string DisplayString
		{
			get
			{
				// rafactor to actually use the builder
				var builder = new StringBuilder();
				builder.Append(_goals.Aggregate("",
					(current, goal) => current + "  " + goal.ProductName));

				return builder.ToString();
			}
		}

		public void AddGoal(ProductGoal goal)
		{
			_goals.Add(goal);
		}
	}
}