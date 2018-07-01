using System;
using System.Collections.Generic;
using Assets.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Assets.WorldMaterials.Products
{
	/// <summary>
	/// Products define the material of the world. 
	/// Recipes have ingredients and a container they can be made in.
	/// </summary>
	
	// used for product transactions
	[Serializable]
	public class ProductAmount
	{
		public int ProductId;
		public int Amount;

		public ProductAmount() { }

	    public ProductAmount(ProductAmount copy)
	    {
	        ProductId = copy.ProductId;
	        Amount = copy.Amount;
	    }

		public ProductAmount(int id, int amount)
		{
			ProductId = id;
			Amount = amount;
		}
	}

	// For sorting? Feels like it could be useful in many instances
	[Serializable]
	public enum ProductCategory
	{
		Raw, Refined, Placeable, Core
	}

	[Serializable]
	public class Ingredient
	{
		public int ProductId;
		public int Quantity;
	}

	[Serializable]
	public class RecipeResult
	{
		public int ProductId;
		public int Quantity;
	}

	[Serializable]
	public class Recipe
	{
		public string DisplayName;
		public List<RecipeResult> Results;
		public List<Ingredient> Ingredients;
		public TimeLength TimeLength;
		public CraftingContainerInfo Container;
		public int Id;
	}

	[Serializable]
	public class CraftingContainerInfo
	{
		public string Name;
		public float CraftingSpeed;
	}

	[Serializable]
	public class Product
	{
		public int ID;
		public string Name;
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductCategory Category;
	    public Color Color;

		public Sprite Icon;

		// Value? If common currency (credits?) is a thing
		// Quality?
	}
}