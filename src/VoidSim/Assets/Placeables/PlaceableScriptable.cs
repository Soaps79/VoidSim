using UnityEditor;
using UnityEngine;

namespace Assets.Station
{
    public class PlaceableScriptable : ScriptableObject
    {
        public string ProductName;
        public Sprite IconSprite;
        public Sprite PlacedSprite;
        public LayerType Layer;
        public Placeable Prefab;

        [MenuItem("Assets/WorldMaterials/Placeable")]
        public static void CreateMyAsset()
        {
            var asset = CreateInstance<PlaceableScriptable>();

            AssetDatabase.CreateAsset(asset, "Assets/Placeables/Resources/NewPlaceable.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
