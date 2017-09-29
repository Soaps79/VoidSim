using System.Linq;
using System.Text;
using Assets.WorldMaterials;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	/// <summary>
	/// Tracks whether or not a quantity of product is contained in station's inventory
	/// </summary>
	public class AccumulateGoalTracker : ProductGoalTrackerBase
	{
		public override GoalType GoalType { get { return GoalType.AccumulateProduct; } }
		private readonly Inventory _inventory;

		public AccumulateGoalTracker()
		{
			_inventory = GameObject.Find("station_inventory").GetComponent<Inventory>();
			_inventory.OnProductsChanged += HandleProductupdate;
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