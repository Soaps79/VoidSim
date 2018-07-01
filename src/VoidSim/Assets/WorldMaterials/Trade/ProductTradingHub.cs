using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.WorldMaterials.Trade
{
	public static class TradeMessages
	{
		public const string TraderCreated = "TraderInstance";
		public const string TradeAccepted = "TradeAccepted";
    }

    /// <summary>
    /// Manages the supply and demand of ProductAmount between game actors. 
    /// Traders are added through messaging, can expose requests to provide or consume
    /// 
    /// Will need work to handle currency
    /// </summary>
    public class ProductTradingHub : QScript, IMessageListener
    {
        private readonly List<ProductTrader> _traders = new List<ProductTrader>();
        
        void Start()
        {
            Locator.MessageHub.AddListener(this, TradeMessages.TraderCreated);
	        SceneManager.sceneLoaded += ClearLists;

            // TODO: re-assess when to tick trades
            var node =  StopWatch.AddNode("CheckForTrades", 5);
            node.OnTick += CheckForTrades;
        }

	    private void ClearLists(Scene arg0, LoadSceneMode arg1)
	    {
		    ClearLists();
	    }

	    public void HandleMessage(string type, MessageArgs args)
        {
            if (type == TradeMessages.TraderCreated && args != null)
                HandleTraderAdd(args as TraderInstanceMessageArgs);
        }

        protected void HandleTraderAdd(TraderInstanceMessageArgs args)
        {
            if(args == null || args.Trader == null)
                throw new UnityException("TradeHub given bad message data");

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
                        var consumed = consumer.Consuming.FirstOrDefault(i => i.ProductId == provided.ProductId);

                        // only consider consumers who want a real amount of this product
                        if (consumed == null || consumed.Amount <= 0 || !IsMatch(provider, consumer, provided))
		                    continue;
                        
                        var amountConsumed = 0;

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
                        if (amountConsumed >= 0)
                        {
                            var info = new TradeManifest
                            {
                                Id = Locator.LastId.GetNext("trade_manifest"),
                                Consumer = consumer.ClientName,
                                Provider = provider.ClientName,
                                AmountTotal = amountConsumed,
                                ProductId = provided.ProductId,
								Status = TradeStatus.Accepted
                            };

                            provider.HandleProvideSuccess(info);
                            consumer.HandleConsumeSuccess(info);

                            // this should be moved out to a driver when this class is made more generic
                            RequestCargo(info);
							Locator.MessageHub.QueueMessage(TradeMessages.TradeAccepted, new TradeCreatedMessageArgs { TradeManifest = info });
                        }

                        // move on to the next consumer, break if provider has emptied its stock
                        totalAmountConsumed += amountConsumed;
                        if (totalAmountConsumed >= totalAmountProvided) break;
                    }
                }
            }

            if (needsPruning) PruneTraderList();
        }

	    private bool IsMatch(ProductTrader provider, ProductTrader consumer, ProductAmount provided)
	    {
				   // eliminate trading with self
			return consumer.ClientName != provider.ClientName
				   // consumers who don't want this product
			       && consumer.Consuming.Any(i => i.ProductId == provided.ProductId)
				   // and traders who won't work with each other
				   && consumer.WillConsumeFrom(provider, provided)
				   && provider.WillProvideTo(consumer, provided);

		}

        private static void RequestCargo(TradeManifest manifest)
        {
            // request cargo for trade
            Locator.MessageHub.QueueMessage(LogisticsMessages.CargoTransitRequested, new CargoTransitRequestedMessageArgs
            {
                Manifest = new CargoManifest
                {
                    Shipper = manifest.Provider,
                    Receiver = manifest.Consumer,
                    ProductId = manifest.ProductId,
                    TotalAmount = manifest.AmountTotal,
                    RemainingAmount = manifest.AmountTotal,
                    Currency = ProductValueLookup.Instance.GetValueOfProductAmount(manifest.ProductId, manifest.AmountTotal)
                }
            });
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