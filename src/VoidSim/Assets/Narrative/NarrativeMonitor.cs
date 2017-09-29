using System.Collections.Generic;
using System.Linq;
using Assets.Narrative.Goals;
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
		private CraftGoalTracker _craftGoalTracker;
		[SerializeField] private List<ProductGoal> _productGoals = new List<ProductGoal>();

		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
		}

		private void Initialize(float obj)
		{
			InitializeProducts();
			InitializeCraftGoalTracker();
		}

		private void InitializeProducts()
		{
			var allProducts = ProductLookup.Instance.GetProducts();
			foreach (var productGoal in _productGoals)
			{
				var product = allProducts.FirstOrDefault(i => i.Name == productGoal.ProductName);
				if (product == null)
					throw new UnityException("NarrativeMonitor given bad product");
				productGoal.ProductId = product.ID;
			}
		}

		private void InitializeCraftGoalTracker()
		{
			_craftGoalTracker = new CraftGoalTracker();
			foreach (var goal in _productGoals)
			{
				_craftGoalTracker.AddGoal(goal);
			}
			KeyValueDisplay.Instance.Add("Goals", () => _craftGoalTracker.DisplayString);
		}
	}
}