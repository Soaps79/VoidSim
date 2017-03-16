using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.Scripts.WorldMaterials
{
    /// <summary>
    /// Products define the material of the world. 
    /// Recipes have ingredients and a container they can be made in.
    /// </summary>

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
        public string ResultProduct;
        public List<Ingredient> Ingredients;
        public TimeLength TimeLength;
        public CraftingContainer Container;
    }

    [Serializable]
    public class CraftingContainer
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

        // Value? If common currency (credits?) is a thing
        // Quality?
    }
}