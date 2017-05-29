    using System;
    using System.Collections.Generic;
    using Assets.Logistics;
    using Assets.Logistics.Ships;
    using Assets.Void;
    using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
    using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Station
{
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

        public void Initialize(Inventory inventory, InventoryReserve reserve)
        {
            _inventory = inventory;
            _inventory.OnInventoryChanged += CheckForTrade;
            _reserve = reserve;

            BindToTrader();
            _valueLookup = ProductValueLookup.Instance;

            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });
        }

        private void BindToTrader()
        {
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.ClientName = ClientName;
            _trader.OnProvideMatch += HandleProvideMatch;
            _trader.OnConsumeMatch += HandleConsumeMatch;
        }

        private void HandleProvideMatch(TradeInfo info)
        {
            // need to add logic to place a hold on the traded items in the reserve
            _reserve.AdjustHold(info.ProductId, -info.AmountTotal);

            // request cargo for trade
            MessageHub.Instance.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
            {
                Manifest = new CargoManifest(info)
                {
                    Seller = ClientName,
                    Buyer = info.Consumer.ClientName,
                    Currency = _valueLookup.GetValueOfProductAmount(info.ProductId, info.AmountTotal),
                    ProductAmount = new ProductAmount { ProductId = info.ProductId, Amount = info.AmountTotal }
                }
            });

            CheckForTrade();
        }

        private void HandleConsumeMatch(TradeInfo info)
        {
			// still need this? Provider does the work
			// reserve the money?
			_reserve.AdjustHold(info.ProductId, info.AmountTotal);
			CheckForTrade();
        }

        private void CheckForTrade()
        {
            var list = _reserve.GetProvideProducts();
            list.ForEach(i => _trader.SetProvide(i));

            list = _reserve.GetConsumeProducts();
            list.ForEach(i => _trader.SetConsume(i));
        }
    }
}