using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.WorldMaterials
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
        public string ResultProduct;
        public List<IngredientInfo> Ingredients;
        public TimeLength TimeLength;
        public string ContainerName;
    }

    [Serializable]
    public class ProductInfo
    {
        public int ID;
        public string Name;
        public ProductCategory Category;
    }

    public class ProductLookupScriptable : ScriptableObject
    {
        public List<ProductInfo> Products;
        public List<CraftingContainerInfo> Containers;
        public List<RecipeInfo> Recipes;

        [MenuItem("Assets/Create/ProductLookup")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<ProductLookupScriptable>();
            AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
