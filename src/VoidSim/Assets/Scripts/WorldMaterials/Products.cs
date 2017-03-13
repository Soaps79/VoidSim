using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.Scripts.WorldMaterials
{
	// Not sure that these enums should be passed throughout the game.
	// They are currently favored as a stepping stone for use in the editor, 
	// which will need custom editor controls. Trying to limit their 
	// proliferation in case // costomizing the editor isn't worth he hassle, 
	// to make it easier to switch to strings and an outside editor

	// Example Recipe: 1 Iron + 3 Basalt in a SmallFactory produce 2 BuildingMaterial

	// Inspiration: Asteroids are made up of oxygen and silicon, the number one and number two most abundant elements in the Earth's crust. 
	// The metallic asteroids are composed of up to 80 % iron and 20 % a mixture of 
	// nickel, iridium, palladium, platinum, gold, magnesium and other precious 
	// metals such as osmium, ruthenium and rhodium.

	[Serializable]
	public enum ProductName
	{
		Oxygen,
		Iron, // plentiful resource
		Nickle, // better buiding material
		Iridium, // energy
		Magnesium, // explosive
		BuildingMaterial, // <-
		PowerCell
	}

	[Serializable]
	public enum ProductionContainerName
	{
		SmallFactory,
		FuelRefinery,
	}

	// For sorting? Feels like it could be useful in many instances
	[Serializable]
	public enum ProductCategory
	{
		Raw, Refined, Luxury
	}

	[Serializable]
	public class IngredientModel
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductName ProductName;
		public int Quantity;
	}

	[Serializable]
	public class RecipeModel
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductName ResultProduct;
		public IngredientModel[] Ingredients;

		[JsonProperty("containers", ItemConverterType = typeof(StringEnumConverter))]
		public ProductionContainerName[] ProductionContainers;
	}

	[Serializable]
	public class ProductModel
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductName Name;
		[JsonConverter(typeof(StringEnumConverter))]
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