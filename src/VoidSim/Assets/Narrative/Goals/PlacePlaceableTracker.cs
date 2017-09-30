using System.Linq;
using Assets.Placeables;
using Assets.Station;
using UnityEngine;

namespace Assets.Narrative.Goals
{
	public class PlacePlaceableTracker : ProductGoalTrackerBase
	{
		public override GoalType GoalType { get { return GoalType.PlacePlaceable; } }


		private readonly PlaceablesMonitor _placeablesMonitor;

		public PlacePlaceableTracker()
		{
			// here be brittle
			_placeablesMonitor = GameObject.Find("core").GetComponent<PlaceablesMonitor>();
			_placeablesMonitor.OnPlaced += HandlePlaced;
		}

		private void HandlePlaced(Placeable placeable)
		{
			//foreach (var goal in Goals)
			//{
			//	if (goal.ProductName == placeable.PlaceableName)
			//	{
			//		goal.TriggerComplete(true);
			//	}

			//}

			//var completed = Goals.Where(i => i.)
		}
	}
}