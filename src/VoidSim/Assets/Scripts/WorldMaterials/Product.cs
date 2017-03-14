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
    /// 
    /// Using enums to support the editor without having to type. Strings would be great 
    /// if there was a way to define products in the editor and then see them in dropdowns.
    /// </summary>
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
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductName ProductName;
        public int Quantity;
    }

    [Serializable]
    public class Recipe
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductName ResultProduct;
        public List<Ingredient> Ingredients;
        public TimeLength TimeLength;

        [JsonProperty("containers", ItemConverterType = typeof(StringEnumConverter))]
        public List<ProductionContainerName> ProductionContainers;
    }

    [Serializable]
    public class Product
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductName Name;
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductCategory Category;

        // Value? If common currency (credits?) is a thing
        // Quality?
    }
}