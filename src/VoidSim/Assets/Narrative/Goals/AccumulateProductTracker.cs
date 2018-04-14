using System.Linq;
using System.Text;
using Assets.WorldMaterials;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Tracks whether or not a quantity of product is contained in station's inventory
	/// </summary>
	public class AccumulateProductTracker : ProductGoalTrackerBase
	{
		public override GoalType GoalType { get { return GoalType.AccumulateProduct; } }
		private readonly ProductInventory _stationInventory;

		public AccumulateProductTracker()
		{
			_stationInventory = GameObject.Find("station_inventory").GetComponent<StationInventory>().Products;
			_stationInventory.OnProductsChanged += HandleProductupdate;
		}

		// set goal elapsed to current inventory level
		// if goal is complete, complete it
		protected override void OnGoalAdded(ProductGoal goal)
		{
			goal.ElapsedAmount = _stationInventory.GetProductCurrentAmount(goal.ProductId);
			if(goal.ElapsedAmount > goal.TotalAmount)
				goal.TriggerComplete(true);
		}
	}
}