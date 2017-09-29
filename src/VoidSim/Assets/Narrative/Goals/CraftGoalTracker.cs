using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Station;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	[Serializable]
	public class ProductGoal
	{
		public string ProductName;
		public int ProductId;
		public int TotalAmount;
		public int ElapsedAmount;
	}

	/// <summary>
	/// This objects hooks into the station's FactoryControl, and responds to any completed craft
	/// by checking if there is a related goal, and progressing that goal accordingly
	/// </summary>
	public class CraftGoalTracker
	{
		private FactoryControl _factoryControl;
		private readonly List<ProductGoal> _productGoals = new List<ProductGoal>();

		public CraftGoalTracker()
		{
			_factoryControl = GameObject.Find("factory_control").GetComponent<FactoryControl>();
			_factoryControl.OnCraftComplete += HandleCraftComplete;
		}

		private void HandleCraftComplete(Recipe recipe)
		{
			var completedGoals = new List<ProductGoal>();
			foreach (var result in recipe.Results)
			{
				var goals = _productGoals.Where(i => i.ProductId == result.ProductId);
				if (!goals.Any())
					continue;

				foreach (var productGoal in goals)
				{
					productGoal.ElapsedAmount += result.Quantity;
					if (productGoal.ElapsedAmount > productGoal.TotalAmount)
						completedGoals.Add(productGoal);
				}
			}

			if (!completedGoals.Any())
				return;

			// tell something the goal is complete
			_productGoals.RemoveAll(i => completedGoals.Contains(i));
		}

		public void AddGoal(ProductGoal goal)
		{
			_productGoals.Add(goal);
		}

		public string DisplayString
		{
			get
			{
				// rafactor to actually use the builder
				var builder = new StringBuilder();
				builder.Append(_productGoals.Aggregate("",
					(current, goal) => current + "  " + goal.ProductName + " " + goal.ElapsedAmount + "/" + goal.TotalAmount));

				return builder.ToString();
			}
		}
	}
}