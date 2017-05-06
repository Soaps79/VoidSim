using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.Station;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Placeables
{

    // These will be moved and evolve alongside the Placeables system
    public static class PlaceableMessages
    {
        public const string PlaceablePlacedMessageName = "PlaceablePlaced";
    }

    public class PlaceablePlacedArgs : MessageArgs
    {
        public Placeable ObjectPlaced;
        public LayerType Layer;
    }

    /// <summary>
    /// Represents any structure or module or any other object placed into the game world.
    /// </summary>
    public class Placeable : QScript
    {
        [HideInInspector] public LayerType Layer;

        private Product _baseProduct;
        private PlaceableScriptable _scriptable;

        public void BindToScriptable(PlaceableScriptable scriptable)
        {
            _scriptable = scriptable;
            Layer = scriptable.Layer;
            _baseProduct = ProductLookup.Instance.GetProduct(scriptable.ProductName);

            gameObject.TrimCloneFromName();
            var rend = gameObject.GetOrAddComponent<SpriteRenderer>();
            rend.enabled = true;
            rend.sprite = scriptable.PlacedSprite;
            rend.sortingLayerName = Layer.ToString();
            rend.sortingOrder = 1;
        }
    }
}