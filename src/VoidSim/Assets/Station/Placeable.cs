using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    /// <summary>
    /// Represents any structure or module or any other object placed into the game world.
    /// </summary>
    public class Placeable : QScript, IEnergyConsumer
    {
        [HideInInspector] public LayerType Layer;

        private Product _baseProduct;
        [SerializeField] private int _energyConsumption;
        private PlaceableScriptable _scriptable;

        // extract energy info to child class?
        public EnergyConsumerNode EnergyConsumerNode { get; private set; }
        private void InitializeEnergyConsumer()
        {
            EnergyConsumerNode = new EnergyConsumerNode();
            EnergyConsumerNode.AmountConsumed = _energyConsumption;
        }

        public void BindToScriptable(PlaceableScriptable scriptable)
        {
            _scriptable = scriptable;
            Layer = scriptable.Layer;
            _baseProduct = ProductLookup.Instance.GetProduct(scriptable.ProductName);

            gameObject.TrimCloneFromName();
            var rend = this.gameObject.AddComponent<SpriteRenderer>();
            rend.sprite = scriptable.PlacedSprite;
            rend.sortingLayerName = Layer.ToString();

            if (_energyConsumption > 0)
                InitializeEnergyConsumer();
        }
    }
}