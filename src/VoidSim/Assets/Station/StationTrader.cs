using System;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    public class TradeSuccessMessageArgs : MessageArgs
    {
        public const string MessageName = "TradeSuccess";
        public ProductTrader Provider;
        public ProductTrader Consumer;
        public int ProductId;
        public int Amount;
        public int Credits;
    }

    /// <summary>
    /// This is currently doing multiple small jobs that will most likely become their own behaviors or systems.
    /// Acts as a driver for ProductTrader, telling it what to buy and sell on the market. 
    /// Ties an Inventory to a ProductTrader, moving products in and out as transactions complete.
    /// Handles broadcasting of successful trades. Currently works because Station is one of 2 actors, involved in every trade.
    ///     As Void develops, trades between other actors will not be broadcast. Not sure if will be relevant.
    ///     This seems like a job that should be handled by TradingHub, but then it has to know about currency, so ???
    /// </summary>
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

            BindToTrader();
            _valueLookup = ProductValueLookup.Instance;

            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

            _creditsProductId = ProductLookup.Instance.GetProduct("Credits").ID;
        }

        private void BindToTrader()
        {
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.OnProvideSuccess += HandleProvideSuccess;
            _trader.OnConsumeSucess += HandleConsumeSuccess;
        }

        private void HandleProvideSuccess(int productId, int amount, ProductTrader consumer)
        {
            _inventory.RemoveProduct(productId, amount);
            var currencyTraded = GetCreditsValue(productId, amount);
            _inventory.TryAddProduct(_creditsProductId, currencyTraded);
            MessageHub.Instance.QueueMessage(
                TradeSuccessMessageArgs.MessageName, new TradeSuccessMessageArgs
                {
                    Provider = _trader,
                    Consumer = consumer,
                    ProductId = productId,
                    Amount = amount,
                    Credits = currencyTraded
                });
        }

        private void HandleConsumeSuccess(int productId, int amount, ProductTrader provider)
        {
            _inventory.TryAddProduct(productId, amount);
            var currencyTraded = GetCreditsValue(productId, amount);
            _inventory.RemoveProduct(_creditsProductId, currencyTraded);

            MessageHub.Instance.QueueMessage(
                TradeSuccessMessageArgs.MessageName, new TradeSuccessMessageArgs
                {
                    Provider = provider,
                    Consumer = _trader,
                    ProductId = productId,
                    Amount = amount,
                    Credits = currencyTraded
                });

        }

        private int GetCreditsValue(int productId, int amount)
        {
            return amount * _valueLookup.GetValueOfProduct(productId);
        }

        private void CheckForTrade(object sender, EventArgs e)
        {
            var list = _reserve.GetProvideProducts();
            list.ForEach(i => _trader.SetProvide(i));

            list = _reserve.GetConsumeProducts();
            list.ForEach(i => _trader.SetConsume(i));
        }
    }
}