using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Placeables
{
    public class PlaceablesLookup : ScriptableObject
    {
        public List<PlaceableScriptable> Placeables;

        [MenuItem("Assets/WorldMaterials/PlaceablesLookup")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<PlaceablesLookup>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Scriptables/NewScripableObject.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
