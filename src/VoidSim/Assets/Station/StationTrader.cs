    using System;
    using System.Collections.Generic;
    using Assets.Logistics;
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
    public class StationTrader : QScript
    {
        private InventoryReserve _reserve;
        public string ClientName { get; set; }

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
            _trader.OnProvideMatch += HandleProvideMatch;
            _trader.OnConsumeMatch += HandleConsumeMatch;
        }

        private void HandleProvideMatch(TradeInfo info)
        {
            // need to add logic to place a hold on the traded items in the reserve

            // request cargo for trade
            MessageHub.Instance.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
            {
                Manifest = new TradeManifest
                {
                    Seller = ClientName,
                    Buyer = info.Consumer.ClientName,
                    Currency = _valueLookup.GetValueOfProductAmount(info.ProductId, info.Amount),
                    Products = new List<ProductAmount> { new ProductAmount { ProductId = info.ProductId, Amount = info.Amount } }
                }
            });

            //Debug.Log(string.Format("Station provide match: {0} {1}", info.ProductId, info.Amount));
        }

        private void HandleConsumeMatch(TradeInfo info)
        {
            // still need this? Provider does the work
        }

        private void CheckForTrade(object sender, EventArgs e)
        {
            var list = _reserve.GetProvideProducts();
            list.ForEach(i => _trader.SetProvide(i));

            list = _reserve.GetConsumeProducts();
            list.ForEach(i => _trader.SetConsume(i));
        }


        // add items to inventory for now, add to traffic eventually
        //public void OnTransitArrival(TransitRegister.Entry entry)
        //{
        //    if (entry.TravelingTo.ClientName != ClientName) return;

        //    var buying = entry.Ship.GetBuyerManifests(ClientName);
        //    foreach (var tradeManifest in buying)
        //    {
        //        foreach (var product in tradeManifest.Products)
        //        {
        //            _inventory.TryAddProduct(product.ProductId, product.Amount);
        //        }
        //        _inventory.TryRemoveProduct(_creditsProductId, tradeManifest.Currency);
        //        entry.Ship.CloseManifest(tradeManifest.Id);
        //    }

        //    var selling = entry.Ship.GetSellerManifests(ClientName);
        //    foreach (var tradeManifest in selling)
        //    {
        //        foreach (var product in tradeManifest.Products)
        //        {
        //            _inventory.TryRemoveProduct(product.ProductId, product.Amount);
        //        }
        //        _inventory.TryAddProduct(_creditsProductId, tradeManifest.Currency);
        //        entry.Ship.CloseManifest(tradeManifest.Id);
        //    }

        //    entry.Ship.CompleteVisit();
        //}
    }
}