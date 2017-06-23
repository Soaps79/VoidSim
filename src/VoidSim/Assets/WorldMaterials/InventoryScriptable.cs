using System;
using System.Collections.Generic;
using Assets.Placeables;
using Assets.WorldMaterials.Products;
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
        public static PlaceablesLookup PlaceablesLookup;
        public static string[] ProductNames;
        public List<string> Placeables;
        public List<ProductCategory> ProductsToIgnore;

        public List<ProductEntryInfo> Products;

        void OnEnable()
        {
            ProductLookup = ScriptableObject.Instantiate(
                Resources.Load("Scriptables/product_lookup")) as ProductLookupScriptable;

            PlaceablesLookup = ScriptableObject.Instantiate(
                Resources.Load("placeables_lookup")) as PlaceablesLookup;
        }
    }
}
