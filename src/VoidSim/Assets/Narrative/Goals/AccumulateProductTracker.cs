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
		private readonly Inventory _inventory;

		public AccumulateProductTracker()
		{
			_inventory = GameObject.Find("station_inventory").GetComponent<Inventory>();
			_inventory.OnProductsChanged += HandleProductupdate;
		}
	}
}