using System;
using System.Collections.Generic;
using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Trade
{
    [Serializable]
    public class TradeRequestInfo
    {
        public TimeUnit Frequency;
        public string Product;
        public int Amount;
        public bool isSelling;
    }

    /// <summary>
    /// Allows editor scripting for frequent trade requests. 
    /// Will possibly be phased out, but enables initial void demands
    /// </summary>
    public class TraderRequestsSO : ScriptableObject
    {
        public List<TradeRequestInfo> Requests;

        public static ProductLookupScriptable ProductLookup { get; set; }

        void OnEnable()
        {
            ProductLookup = ScriptableObject.Instantiate(
                Resources.Load("Scriptables/product_lookup")) as ProductLookupScriptable;
        }

        [MenuItem("Assets/WorldMaterials/TraderRequests")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<TraderRequestsSO>();

            AssetDatabase.CreateAsset(asset, "Assets/Resources/Scriptables/NewTradeRequestSO.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}