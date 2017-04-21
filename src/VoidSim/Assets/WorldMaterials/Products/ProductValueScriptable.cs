using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Products
{
    [Serializable]
    public class ProductValueInfo
    {
        public string ProductName;
        public int Value;
    }

    public class ProductValueScriptable : ScriptableObject
    {
        public List<ProductValueInfo> Products;
        public static ProductLookupScriptable ProductLookup;

        void OnEnable()
        {
            ProductLookup = ScriptableObject.Instantiate(
                Resources.Load("Scriptables/product_lookup")) as ProductLookupScriptable;

        }

        [MenuItem("Assets/WorldMaterials/ProductValue")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<ProductValueScriptable>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Scriptables/NewProductValue.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}