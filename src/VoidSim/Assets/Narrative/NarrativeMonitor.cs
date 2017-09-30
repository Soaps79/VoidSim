using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.Narrative.Missions;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Narrative
{
	/// <summary>
	/// Orchestrate the Narrative. Will probably end up handling too many duties, and jobs will be broken out.
	/// </summary>
	public class NarrativeMonitor : QScript
	{
		// this is so ugly, extract an interface pls
		private List<ProductGoalTrackerBase> _trackers = new List<ProductGoalTrackerBase>();
		[SerializeField] private List<Mission> _initialMissions = new List<Mission>();

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
		}

		private void Initialize(float obj)
		{
			_initialMissions.ForEach(i => i.Initialize());
			InitializeProducts();
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();
		}

		private void InitializeProducts()
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var mission in _initialMissions)
			{
				foreach (var productGoal in mission.Goals)
				{
					var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
					if (product == null)
						throw new UnityException("NarrativeMonitor given bad product");
					productGoal.ProductId = product.ID;
				}
			}
		}

		private void InitializeCraftProductTracker()
		{
			var tracker = new CraftProductTracker();
			AddInitialGoalsToTracker(tracker);
			KeyValueDisplay.Instance.Add("Make", () => tracker.DisplayString);
		}
		private void InitializeAccumulateProductTracker()
		{
			var tracker = new AccumulateProductTracker();
			AddInitialGoalsToTracker(tracker);
			KeyValueDisplay.Instance.Add("Have", () => tracker.DisplayString);
		}

		private void InitializeSellProductTracker()
		{
			var tracker = new SellProductTracker();
			AddInitialGoalsToTracker(tracker);
			KeyValueDisplay.Instance.Add("Sell", () => tracker.DisplayString);
		}

		// finds initial goals of a tracker's type and hends them off
		private void AddInitialGoalsToTracker(ProductGoalTrackerBase tracker)
		{
			var missions = _initialMissions.Where(i => i.Goals.Any(j => j.Type == tracker.GoalType)).ToList();

			if (!missions.Any())
				return;

			foreach (var mission in missions)
			{
				foreach (var goal in mission.Goals.Where(i => i.Type == tracker.GoalType))
				{
					tracker.AddGoal(goal);
				}
			}
		}

	}
}