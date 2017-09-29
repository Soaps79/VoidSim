using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Station;
using Assets.WorldMaterials.Products;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// This objects hooks into the station's FactoryControl, and responds to any completed craft
	/// by checking if there is a related goal, and progressing that goal accordingly
	/// </summary>
	public class CraftGoalTracker : ProductGoalTrackerBase
	{
		public override GoalType GoalType { get { return GoalType.CraftProduct; } }
		private readonly FactoryControl _factoryControl;

		public CraftGoalTracker()
		{
			_factoryControl = GameObject.Find("factory_control").GetComponent<FactoryControl>();
			_factoryControl.OnCraftComplete += HandleCraftComplete;
		}

		private void HandleCraftComplete(Recipe recipe)
		{
			foreach (var result in recipe.Results)
			{
				HandleProductupdate(result.ProductId, result.Quantity);
			}
		}

		// not bothering extracting, probably temporary
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