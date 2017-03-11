using System;
using System.Collections.Generic;

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
		public class Ingredient
		{
			public ProductName ProductName;
			public int Quantity;
		}

		[Serializable]
		public class Recipe
		{
			public ProductName ResultProduct;
			public Ingredient[] Ingredients;
			public ProductionContainerName[] ProductionContainers;
		}

		[Serializable]
		public class Product
		{
			public ProductName Name;
			public ProductCategory Category;

		// Value? If common currency (credits?) is a thing
		// Quality?
	}

	[Serializable]
		public class ProductEditorView
		{
			public Product Product;
			public Recipe[] Recipes;
		}
}