using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    public class StationTrader : QScript
    {
        private InventoryReserve _reserve;

        [SerializeField] private ProductValueLookup _valueLookup;
        [SerializeField] private Inventory _inventory;
        private ProductTrader _trader;
        private WorldClock _worldClock;
        private int _creditsProductId;

        public void Initialize(Inventory inventory, InventoryReserve reserve)
        {
            _inventory = inventory;
            _worldClock = WorldClock.Instance;
            _worldClock.OnDayUp += CheckForTrade;
            _reserve = reserve;
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.OnProvideSuccess += HandleProvideSuccess;
            _valueLookup = ProductValueLookup.Instance;

            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

            _creditsProductId = ProductLookup.Instance.GetProduct("Credits").ID;
        }

        private void HandleProvideSuccess(int productId, int amount)
        {
            _inventory.RemoveProduct(productId, amount);
            var value = _valueLookup.GetValueOfProduct(productId);
            _inventory.TryAddProduct(_creditsProductId, amount * value);
        }

        private void CheckForTrade(object sender, EventArgs e)
        {
            var excess = _reserve.GetReleaseProducts();
            excess.ForEach(i => _trader.SetProvide(i));
        }
    }
}