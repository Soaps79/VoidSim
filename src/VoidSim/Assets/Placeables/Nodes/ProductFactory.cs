﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Scripts.WorldMaterials;
using Assets.Station.Efficiency;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using UnityEngine;

// some Station owned object(Station) holds a reference to this
namespace Assets.Placeables.Nodes
{
	public class ProductFactoryMessageArgs : MessageArgs
	{
		public ProductFactory ProductFactory;
	}

	public class ProductFactoryData
	{
		public string Name;
		public bool IsCrafting;
		public int CurrentlyCraftingRecipeId;
		public float RemainingCraftTime;
		public bool IsBuying;
	}

	/// <summary>
	/// Gives placeable an automated crafting container
	/// </summary>
	[RequireComponent(typeof(Placeable))]
	[RequireComponent(typeof(EfficiencyNode))]
	public class ProductFactory : PlaceableNode<ProductFactory>, ISerializeData<ProductFactoryData>
	{
	    protected override ProductFactory GetThis() { return this; }
		public override string NodeName { get { return "ProductFactory"; } }
		public const string MessageName = "ProductFactoryPlaced";

		// *** This object was refactored to use ID's instead of strings
		// *** Should probably get a cleanup pass
		
		[SerializeField] private string _containerType;
		private readonly List<Recipe> _recipes = new List<Recipe>();
		private int _currentCraftQueueId;
		private ProductInventory _stationInventory;
		public EfficiencyModule EfficiencyModule { get; private set; }

		// these next fields assigned in unity editor / prefab
		public bool IsInPlayerArray;
		public string InitialRecipe;
		[SerializeField] private bool _isCore;
		public bool IsCore { get { return _isCore; } }

		public Recipe CurrentlyCrafting { get; private set; }
		public List<Recipe> Recipes { get { return _recipes; } }
		public bool IsCrafting { get; private set; }

		private ProductLookup _productLookup;
		private CraftingContainer _container;

		private bool _isOutOfProduct;
		public bool IsBuying { get; private set; }
		public Action OnIsBuyingchanged;
		public Action<Recipe> OnCraftComplete;

	    public override void Initialize(PlaceableData data)
	    {
	        EfficiencyModule = GetComponent<EfficiencyNode>().Module;
	        EfficiencyModule.OnValueChanged += OnEfficiencyChanged;
        }

	    public override void BroadcastPlacement()
		{
			Locator.MessageHub.QueueMessage(MessageName, new ProductFactoryMessageArgs { ProductFactory = this });
		}

		public float CurrentCraftRemainingAsZeroToOne()
		{
			return _container.CurrentCraftElapsedAsZeroToOne;
		}

		// Factory binds to Inventory and initializes its container. Will begin crafting if InitialRecipe not null
		public void Initialize(ProductInventory stationInventory, ProductLookup productLookup)
		{
			_stationInventory = stationInventory;
			_stationInventory.OnProductsChanged += CheckForRestart;

			_productLookup = productLookup;
			if (_container == null)
			{
				_container = gameObject.GetOrAddComponent<CraftingContainer>();
				_container.Info = ProductLookup.Instance.GetContainer(_containerType);
				_container.OnCraftingComplete += StoreProductAndRestartCrafting;
				_container.CurrentEfficiency = EfficiencyModule.CurrentAmount;
			}

			LoadRecipes();
			if (!string.IsNullOrEmpty(InitialRecipe))
			{
				var recipe = _recipes.FirstOrDefault(i => i.DisplayName == InitialRecipe);
				if (recipe == null)
					throw new UnityException(string.Format("ProductFactory initialized with recipe it doesnt have: {0}", InitialRecipe));
				SwitchCurrentCraftingTo(recipe);
			}
		}

		private void OnEfficiencyChanged(EfficiencyModule module)
		{
			if(_container != null)
				_container.CurrentEfficiency = module.CurrentAmount;
		}

		public List<ProductAmount> GetDayForecast()
		{
			var list = new List<ProductAmount>();
			if (CurrentlyCrafting.TimeLength.TimeUnit == TimeUnit.Hour)
			{
				var multiplier = Locator.WorldClock.HoursPerDay / CurrentlyCrafting.TimeLength.Length;
				foreach (var ingredient in CurrentlyCrafting.Ingredients)
				{
					list.Add(new ProductAmount{ ProductId = ingredient.ProductId, Amount = ingredient.Quantity * multiplier });
				}
			}
			else if (CurrentlyCrafting.TimeLength.TimeUnit == TimeUnit.Day)
			{
				foreach (var ingredient in CurrentlyCrafting.Ingredients)
				{
					list.Add(new ProductAmount { ProductId = ingredient.ProductId, Amount = ingredient.Quantity });
				}
			}
			return list;
		}

		public void SetIsBuying(bool value)
		{
			if (IsBuying == value)
				return;

			IsBuying = value;
			if (OnIsBuyingchanged != null)
				OnIsBuyingchanged();
		}

		// called when inventory changes, will restart a container if it stopped
		// because product ran out and that has been remedied
		private void CheckForRestart(int productId, int amount)
		{
			if (!_isOutOfProduct || 
				CurrentlyCrafting.Ingredients.Any(i => !_stationInventory.HasProduct(i.ProductId, productId)))
				return;

			_isOutOfProduct = false;
			SwitchCurrentCraftingTo(CurrentlyCrafting);
		}

		// loads recipes for specified container type from product lookup
		private void LoadRecipes()
		{
			var recipes = _productLookup.GetRecipesForContainer(_containerType);
			if (!recipes.Any())
				throw new UnityException(string.Format("No recipes found for container {0}", _containerType));

			_recipes.Clear();
			_recipes.AddRange(recipes);
		}

		// public version, will switch recipes if it is already crafting one
		public void SwitchCurrentCraftingTo(Recipe recipe)
		{
			if(IsCrafting)
				StopCrafting();

			var existing = _recipes.FirstOrDefault(i => i.DisplayName == recipe.DisplayName);
			if(existing == null)
				throw new UnityException(string.Format("ProductFactory asked to make product that it has no recipe for: {0}", recipe.DisplayName));

			if(_currentCraftQueueId > 0)
				_container.CancelCrafting(_currentCraftQueueId);
			StartCrafting(recipe);
		}

		// stops crafting recipe and refunds ingredients
		public void StopCrafting()
		{
			if (!IsCrafting)
				return;

			RefundProducts();
			_container.CancelCrafting(_currentCraftQueueId);
			IsCrafting = false;
		}

		// puts products back into inventory
		private void RefundProducts()
		{
			foreach (var ingredient in CurrentlyCrafting.Ingredients)
			{
				_stationInventory.TryAddProduct(ingredient.ProductId, ingredient.Quantity);
			}
		}

		// enables basic loop for continuous crafting
		private void StoreProductAndRestartCrafting(Recipe recipe)
		{
			StoreResult(recipe);
            StartCrafting(recipe);
            if (OnCraftComplete != null)
				OnCraftComplete(recipe);
		}

		// handles updating private data
		private void StartCrafting(Recipe recipe)
		{
			if (!TryWithdrawProducts(recipe))
			{
				_isOutOfProduct = true;
				return;
			}

			_isOutOfProduct = false;
			_currentCraftQueueId = _container.QueueCrafting(recipe);
			CurrentlyCrafting = recipe;
			IsCrafting = true;
		}

		// if inventory has enough product for ingredients, remove them
		private bool TryWithdrawProducts(Recipe recipe)
		{
			foreach (var ingredient in recipe.Ingredients)
			{
				if (_stationInventory.HasProduct(ingredient.ProductId, ingredient.Quantity))
					continue;

				Debug.Log(string.Format("Automated container ran out of Product {0}", ingredient.ProductId));
				return false;
			}

			foreach (var ingredient in recipe.Ingredients)
			{
				_stationInventory.TryRemoveProduct(ingredient.ProductId, ingredient.Quantity);
			}
			return true;
		}

		// add successful craft result to inventory
		private void StoreResult(Recipe recipe)
		{
			foreach (var result in recipe.Results)
			{
				_stationInventory.TryAddProduct(result.ProductId, result.Quantity);
			}
		}

		// resumes crafting on game load
		public void Resume(ProductFactoryData data)
		{
			IsBuying = data.IsBuying;
			if (data.IsCrafting)
			{
				var recipe = _productLookup.GetRecipe(data.CurrentlyCraftingRecipeId);
				_currentCraftQueueId = _container.ResumeCrafting(recipe, data.RemainingCraftTime);
				CurrentlyCrafting = recipe;
				IsCrafting = true;
			}
		}

		// returns data for serialization
		public ProductFactoryData GetData()
		{
			return new ProductFactoryData
			{
				Name = InstanceName,
				IsCrafting = IsCrafting,
				IsBuying = IsBuying,
				CurrentlyCraftingRecipeId = IsCrafting ? CurrentlyCrafting.Id : 0,
				RemainingCraftTime = _container.CurrentCraftElapsedAsZeroToOne
			};
		}
	}
}
