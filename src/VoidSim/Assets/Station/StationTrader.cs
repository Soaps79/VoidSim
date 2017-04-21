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
    /// <summary>
    /// Attaches to an Inventory and has a list of product minimums to keep.
    /// Can withdraw and return product counts above the minimum.
    /// </summary>
    public class InventoryReserve
    {
        public Inventory Inventory;
        private List<ProductAmount> _reserveEntries = new List<ProductAmount>();

        public List<ProductAmount> WithdrawExcessProducts()
        {
            var products = new List<ProductAmount>();

            foreach (var productAmount in _reserveEntries)
            {
                var difference = Inventory.GetProductCurrentAmount(productAmount.ProductId) - productAmount.Amount;
                if (difference <= 0) continue;

                Inventory.RemoveProduct(productAmount.ProductId, difference);
                products.Add(new ProductAmount {ProductId = productAmount.ProductId, Amount = difference});
            }

            return products;
        }

        // currently, calling this with an id and 0 can be used to flag a product to always sell
        public void AddReservation(int productId, int amount)
        {
            _reserveEntries.Add(new ProductAmount(productId, amount));
        }
    }

    public class StationTrader : QScript
    {
        private InventoryReserve _reserve;

        [SerializeField] private ProductValueLookup _valueLookup;
        [SerializeField] private Inventory _inventory;
        private ProductTrader _trader;
        private WorldClock _worldClock;
        private Product _creditsProduct;

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
            _worldClock = WorldClock.Instance;
            _worldClock.OnDayUp += CheckForTrade;
            _reserve = new InventoryReserve { Inventory = _inventory };
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.OnProvideSuccess += HandleProvideSuccess;
            _valueLookup = ProductValueLookup.Instance;

            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

            var product = ProductLookup.Instance.GetProduct("Ammunition");
            _reserve.AddReservation(product.ID, 0);
            _creditsProduct = ProductLookup.Instance.GetProduct("Credits");
        }

        private void HandleProvideSuccess(int productId, int amount)
        {
            var value = _valueLookup.GetValueOfProduct(productId);
            _inventory.TryAddProduct(_creditsProduct, amount * value);
        }

        private void CheckForTrade(object sender, EventArgs e)
        {
            var excess = _reserve.WithdrawExcessProducts();
            excess.ForEach(i => _trader.AddProvide(i));
        }
    }
}