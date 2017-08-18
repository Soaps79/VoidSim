using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.Products
{
    /// <summary>
    /// This scriptable and its Info objects are an abstraction of the
    /// Product aimed at the inspector and serialization in general
    /// </summary>
    [Serializable]
    public class IngredientInfo
    {
        public string ProductName;
        public int Quantity;
    }

    [Serializable]
    public class RecipeInfo
    {
	    public string DisplayName;
        public string ResultProduct;
        public int ResultAmount;
        public List<IngredientInfo> Ingredients;
        public TimeLength TimeLength;
        public string ContainerName;
    }

    [Serializable]
    public class ProductInfo
    {
        public int ID;
        public string Name;
        public Sprite IconSprite;
        public ProductCategory Category;
    }

    public class ProductLookupScriptable : ScriptableObject
    {
        public List<ProductInfo> Products;
        public List<CraftingContainerInfo> Containers;
        public List<RecipeInfo> Recipes;
        public Sprite DefaultSmallIcon;

        public string[] GenerateProductNames()
        {
            return Products != null && Products.Any()
                ? Products.Select(i => i.Name).ToArray()
                : new string[1] { "Empty" };
        }

        public string[] GenerateContainerNames()
        {
            return Containers != null && Containers.Any()
                ? Containers.Select(i => i.Name).ToArray()
                : new string[1] { "Empty" };
        }

    }
}
