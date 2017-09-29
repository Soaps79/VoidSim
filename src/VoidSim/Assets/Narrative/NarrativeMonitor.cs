using System;
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
		private CraftProductTracker _craftProductTracker;
		[SerializeField] private List<Mission> _initialMissions = new List<Mission>();
		private AccumulateProductTracker _accumulateProductTracker;
		private SellProductTracker _sellProductTracker;

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
			_craftProductTracker = new CraftProductTracker();
			AddInitialGoalsToTracker(_craftProductTracker);
			KeyValueDisplay.Instance.Add("Make", () => _craftProductTracker.DisplayString);
		}
		private void InitializeAccumulateProductTracker()
		{
			_accumulateProductTracker = new AccumulateProductTracker();
			AddInitialGoalsToTracker(_accumulateProductTracker);
			KeyValueDisplay.Instance.Add("Have", () => _accumulateProductTracker.DisplayString);
		}

		private void InitializeSellProductTracker()
		{
			_sellProductTracker = new SellProductTracker();
			AddInitialGoalsToTracker(_sellProductTracker);
			KeyValueDisplay.Instance.Add("Sell", () => _sellProductTracker.DisplayString);
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