using Assets.Station;
using Assets.WorldMaterials.Products;
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
    }
}
