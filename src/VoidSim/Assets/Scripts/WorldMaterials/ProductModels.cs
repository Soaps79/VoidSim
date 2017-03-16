using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.Scripts.WorldMaterials
{
	/// <summary>
	/// Editor friendly data model
	/// Should only be modified to work better with the editor
	/// </summary>

	// Inspiration: Asteroids are made up of oxygen and silicon, the number one and number two most abundant elements in the Earth's crust. 
	// The metallic asteroids are composed of up to 80 % iron and 20 % a mixture of 
	// nickel, iridium, palladium, platinum, gold, magnesium and other precious 
	// metals such as osmium, ruthenium and rhodium.

	[Serializable]
	public class IngredientModel
	{
		public string ProductName;
		public int Quantity;
	}

	[Serializable]
	public class RecipeModel
	{
		public IngredientModel[] Ingredients;
		public TimeLength TimeLength;

		public string[] ProductionContainers;
	}

	[Serializable]
	public class ProductModel
	{
		public string Name;
		public ProductCategory Category;

		// Value? If common currency (credits?) is a thing
		// Quality?
	}

	[Serializable]
	public class ProductEditorView
	{
		public ProductModel Product;
		public RecipeModel[] Recipes;
	}
}