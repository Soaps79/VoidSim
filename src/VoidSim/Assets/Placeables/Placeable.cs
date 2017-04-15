using System.Collections.Generic;
using Assets.Placeables;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{

    // These will be moved and evolve alongside the Placeables system
    public static class PlaceableMessages
    {
        public const string PlaceablePlacedMessageName = "PlaceablePlaced";
    }

    public class PlaceablePlacedArgs : MessageArgs
    {
        public Placeable ObjectPlaced;
    }

    /// <summary>
    /// Represents any structure or module or any other object placed into the game world.
    /// </summary>
    public class Placeable : QScript
    {
        public List<PlaceableNode> Nodes;

        [HideInInspector] public LayerType Layer;

        private Product _baseProduct;
        private PlaceableScriptable _scriptable;

        public void BindToScriptable(PlaceableScriptable scriptable)
        {
            _scriptable = scriptable;
            Layer = scriptable.Layer;
            _baseProduct = ProductLookup.Instance.GetProduct(scriptable.ProductName);

            gameObject.TrimCloneFromName();
            var rend = this.gameObject.AddComponent<SpriteRenderer>();
            rend.sprite = scriptable.PlacedSprite;
            rend.sortingLayerName = Layer.ToString();
            rend.sortingOrder = 1;
        }
    }
}