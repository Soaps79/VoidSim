using System;
using System.Collections.Generic;

namespace VoidSim.Console.Products
{
	// Example Recipe: 1 Iron + 3 Basalt in a SmallFactory produce 2 BuildingMaterial

	// They are made up of oxygen and silicon, the number one and number two most abundant elements in the Earth's crust. 
	// The metallic asteroids are composed of up to 80 % iron and 20 % a mixture of 
	// nickel, iridium, palladium, platinum, gold, magnesium and other precious 
	// metals such as osmium, ruthenium and rhodium.

	// Writing this knowing how ugly a bunch of enums can be. Doing it this way will enable
	// pre-populated fields in the editor. The goal is to make some cool custom editor controls for Products.
	// If it is hell, it'll be painfully replaced with strings
	[Serializable]
	public enum ProductName
	{
		Oxygen,
		Iron, // plentiful resource
		Nickle, // better buiding material
		Iridium, // energy
		Magnesium, // explosive
		BuildingMaterial, // <-
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
        public string ProductName;
        public int Quantity;
    }

	[Serializable]
	public class Recipe
    {
	    public ProductName Name;
        public Ingredient[] Ingredients;
		public ProductionContainerName[] ProductionContainers;
	}

	[Serializable]
    public class Product
    {
        public ProductCategory Category;
        public ProductName Name;

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