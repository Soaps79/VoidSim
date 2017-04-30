using Assets.Station;
using Assets.WorldMaterials.Products;
using UnityEditor;
using UnityEngine;

namespace Assets.Placeables
{
    public class PlaceableScriptable : ScriptableObject
    {
        public string ProductName;
        public Sprite IconSprite;
        public Sprite PlacedSprite;
        public LayerType Layer;
        public Placeable Prefab;

        public static ProductLookupScriptable ProductLookup;

        void OnEnable()
        {
            ProductLookup = ScriptableObject.Instantiate(
                Resources.Load("Scriptables/product_lookup")) as ProductLookupScriptable;
        }

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
