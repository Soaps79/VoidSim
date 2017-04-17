using System.Collections.Generic;
using System.Linq;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.WorldMaterials
{
    public class TraderInstanceMessageArgs : MessageArgs
    {
        public ProductTrader Trader;
    }

    public class ProductTradeRequest
    {
        public int ProductId;
        public int Amount;
    }

    /// <summary>
    /// Manages the supply and demand of Products between game actors. 
    /// Traders are added through messaging, can expose requests to provide or consume
    /// 
    /// Will need work to handle currency
    /// </summary>
    public class ProductTradingHub : QScript, IMessageListener
    {
        private readonly List<ProductTrader> _traders = new List<ProductTrader>();

        void Start()
        {
            MessageHub.Instance.AddListener(this, ProductTrader.MessageName);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == ProductTrader.MessageName && args != null)
                HandleTraderAdd(args as TraderInstanceMessageArgs);
        }

        protected void HandleTraderAdd(TraderInstanceMessageArgs args)
        {
            if(args == null || args.Trader == null)
                throw new UnityException("TradeHub given bad message data");

            if(_traders.All(i => i.name != args.Trader.name))
                _traders.Add(args.Trader);
        }

        protected void CheckForTrades()
        {
            var providers = _traders.Where(i => i.Providing.Any()).ToList();
            var consumers = _traders.Where(i => i.Consuming.Any()).ToList();

            // will be set to true if any provide or consume request is completed
            var needsPruning = false;

            // loop through all products provided and see if they have consumers
            foreach (var provider in providers)
            {
                foreach (var provided in provider.Providing)
                {
                    var totalAmountConsumed = 0;
                    var totalAmountProvided = provided.Amount;

                    foreach (var consumer in consumers)
                    {
                        // only consider consumers who want this product
                        if (consumer.name == provider.name
                            || consumer.Consuming.All(i => i.ProductId != provided.ProductId)) continue;
                        
                        var amountConsumed = 0;
                        var consumed = consumer.Consuming.First(i => i.ProductId == provided.ProductId);

                        // consumer either finishes off entire amount
                        if (consumed.Amount > provided.Amount)
                        {
                            amountConsumed = provided.Amount;
                            consumed.Amount -= amountConsumed;
                            provided.Amount = 0;
                            needsPruning = true;
                        }
                        // or takes its fill
                        else
                        {
                            amountConsumed = consumed.Amount;
                            provided.Amount -= amountConsumed;
                            consumed.Amount = 0;
                            needsPruning = true;
                        }

                        // tell the consumer it got its goods
                        if(amountConsumed >= 0)
                            consumer.HandleConsumeSuccess(provided.ProductId, amountConsumed);

                        // move on to the next consumer, break if provider has emptied its stock
                        totalAmountConsumed += amountConsumed;
                        if (totalAmountConsumed >= totalAmountProvided) break;
                    }
                    
                    // only tell the provider once how much has been consumed
                    if (totalAmountConsumed > 0)
                    {
                        provider.HandleProvideSuccess(provided.ProductId, totalAmountConsumed);
                    }

                }
            }

            if (needsPruning) PruneTraderList();
        }

        private void PruneTraderList()
        {
            foreach (var trader in _traders)
            {
                trader.Consuming.RemoveAll(i => i.Amount <= 0);
                trader.Providing.RemoveAll(i => i.Amount <= 0);
            }
        }

        protected void ClearLists()
        {
            _traders.Clear();
        }

        public string Name { get { return "ProductTradingHub"; } }
    }
}