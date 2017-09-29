using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Narrative
{
	[Serializable]
	public enum GoalType
	{
		CraftProduct, AccumulateProduct, SellProduct
	}

	// currently, all goals deal with products, write new code knowing this probably won't always be true
	[Serializable]
	public class ProductAmountGoal
	{
		public string ProductName;
		public int ProductId;
		public int TotalAmount;
		public int ElapsedAmount;
		public GoalType Type;
		public bool IsComplete;
		public Action<ProductAmountGoal> OnCompleteChange;

		// if change is true, set IsComplete to it and tell listeners
		public void TriggerComplete(bool isComplete)
		{
			if (IsComplete == isComplete)
				return;

			IsComplete = isComplete;
			if (OnCompleteChange != null)
				OnCompleteChange(this);
		}
	}

	/// <summary>
	/// Orchestrate the Narrative. Will probably end up handling too many duties, and jobs will be broken out.
	/// </summary>
	public class NarrativeMonitor : QScript
	{
		private CraftProductTracker _craftProductTracker;
		[SerializeField] private List<ProductAmountGoal> _initialProductGoals = new List<ProductAmountGoal>();
		private AccumulateProductTracker _accumulateProductTracker;
		private SellProductTracker _sellProductTracker;

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
		}

		private void Initialize(float obj)
		{
			InitializeProducts();
			InitializeCraftProductTracker();
			InitializeAccumulateProductTracker();
			InitializeSellProductTracker();
		}

		private void InitializeProducts()
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var productGoal in _initialProductGoals)
			{
				var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
				if (product == null)
					throw new UnityException("NarrativeMonitor given bad product");
				productGoal.ProductId = product.ID;
			}
		}

		private void InitializeCraftProductTracker()
		{
			_craftProductTracker = new CraftProductTracker();
			AddGoalsToTracker(_craftProductTracker);
			KeyValueDisplay.Instance.Add("Make", () => _craftProductTracker.DisplayString);
		}
		private void InitializeAccumulateProductTracker()
		{
			_accumulateProductTracker = new AccumulateProductTracker();
			AddGoalsToTracker(_accumulateProductTracker);
			KeyValueDisplay.Instance.Add("Have", () => _accumulateProductTracker.DisplayString);
		}

		private void InitializeSellProductTracker()
		{
			_sellProductTracker = new SellProductTracker();
			AddGoalsToTracker(_sellProductTracker);
			KeyValueDisplay.Instance.Add("Sell", () => _sellProductTracker.DisplayString);
		}

		// finds initial goals of a tracker's type and hends them off
		private void AddGoalsToTracker(ProductGoalTrackerBase tracker)
		{
			var goals = _initialProductGoals.Where(i => i.Type == tracker.GoalType);
			if (!goals.Any())
				return;

			foreach (var goal in goals)
			{
				tracker.AddGoal(goal);
			}
		}

	}
}