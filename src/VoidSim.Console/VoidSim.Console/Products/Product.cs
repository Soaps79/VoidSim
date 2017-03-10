using System.Collections.Generic;

namespace VoidSim.Console.Products
{
    // Example Recipe: 1 Iron + 3 Basalt in a SmallFactory produce 2 BuildingMaterial
    
    // For sorting? Feels like it could be useful in many instances
    public enum ProductCategory
    {
        Raw, Refined, Luxury
    }

    public class Ingredient
    {
        public string ProductName;
        public int Quantity;
    }

    public class Recipe
    {
        public string ProductName;
        public string ProductionContainerName;
        public List<Ingredient> Ingredients;
    }

    public class Product
    {
        public ProductCategory Category;
        public string Name;

        // Value? If common currency (credits?) is a thing
        // Quality?

    }
}