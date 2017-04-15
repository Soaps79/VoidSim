using System;
using System.Collections.Generic;
using Assets.Scripts.WorldMaterials;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials
{
    [Serializable]
    public class ProductEntryInfo
    {
        public string ProductName;
        public int Amount;
    }

    public class InventoryScriptable : ScriptableObject
    {
        public int ProductMaxAmount;
        public static ProductLookupScriptable ProductLookup;
        public static string[] ProductNames;
        public List<string> Placeables;
        public List<ProductCategory> ProductsToIgnore;

        public List<ProductEntryInfo> Products;

        void OnEnable()
        {
            ProductLookup = ScriptableObject.Instantiate(
                Resources.Load("Scriptables/product_lookup")) as ProductLookupScriptable;

        }

        [MenuItem("Assets/WorldMaterials/Inventory")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<InventoryScriptable>();

            AssetDatabase.CreateAsset(asset, "Assets/Resources/Scriptables/NewScripableObject.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

    }
}
