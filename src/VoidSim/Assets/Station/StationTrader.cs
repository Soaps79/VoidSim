    using System;
    using Assets.Void;
    using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    public enum TradeStatus
    {
        Accepted, Rejected, Complete
    }

    public class TradeStatusChangedMessageArgs : MessageArgs
    {
        public const string MessageName = "TradeSuccess";
        public ProductTrader Provider;
        public ProductTrader Consumer;
        public int ProductId;
        public int Amount;
        public int Credits;
        public TradeStatus Status;
    }

    /// <summary>
    /// This is currently doing multiple small jobs that will most likely become their own behaviors or systems.
    /// Acts as a driver for ProductTrader, telling it what to buy and sell on the market. 
    /// Ties an Inventory to a ProductTrader, moving products in and out as transactions complete.
    /// Handles broadcasting of successful trades. Currently works only because Station is one of 2 actors, involved in every trade.
    ///     As Void develops, trades between other actors will not be broadcast. Not sure if will be relevant.
    ///     This seems like a job that should be handled by TradingHub, but then it has to know about currency, so ???
    /// </summary>
    public class StationTrader : QScript, ITransitLocation
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
            MessageHub.Instance.QueueMessage(TransitMessages.RegisterLocation, new TransitLocationMessageArgs { TransitLocation = this });

            _creditsProductId = ProductLookup.Instance.GetProduct("Credits").ID;
        }

        private void BindToTrader()
        {
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.OnProvideMatch += HandleProvideMatch;
            _trader.OnConsumeMatch += HandleConsumeMatch;
        }

        private void HandleProvideMatch(TradeInfo info)
        {
            _inventory.TryRemoveProduct(info.ProductId, info.Amount);
            var currencyTraded = GetCreditsValue(info.ProductId, info.Amount);
            _inventory.TryAddProduct(_creditsProductId, currencyTraded);
            MessageHub.Instance.QueueMessage(
                TradeStatusChangedMessageArgs.MessageName, new TradeStatusChangedMessageArgs
                {
                    Provider = _trader,
                    Consumer = info.Consumer,
                    ProductId = info.ProductId,
                    Amount = info.Amount,
                    Credits = currencyTraded,
                    Status = TradeStatus.Accepted
                });
        }

        private void HandleConsumeMatch(TradeInfo info)
        {
            _inventory.TryAddProduct(info.ProductId, info.Amount);
            var currencyTraded = GetCreditsValue(info.ProductId, info.Amount);
            _inventory.TryRemoveProduct(_creditsProductId, currencyTraded);

            MessageHub.Instance.QueueMessage(
                TradeStatusChangedMessageArgs.MessageName, new TradeStatusChangedMessageArgs
                {
                    Provider = info.Provider,
                    Consumer = _trader,
                    ProductId = info.ProductId,
                    Amount = info.Amount,
                    Credits = currencyTraded,
                    Status = TradeStatus.Accepted
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

        public string ClientName { get; set; }

        // add items to inventory for now, add to traffic eventually
        public void OnTransitComplete(TransitRegister.Entry entry)
        {
            if (entry.TravelingTo.ClientName == ClientName)
            {
                foreach (var productAmount in entry.Ship.ProductCargo)
                {
                    _inventory.TryAddProduct(productAmount.ProductId, productAmount.Amount);
                }
            }
        }
    }
}