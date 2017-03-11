using System.Collections.Generic;
using System.Linq;

namespace VoidSim.Console.Products
{
    // This object is essentially a service to the rest of the game whose role is to connect consumers with providers. 
    // The goods do not actually pass through here, Providers simply agree to send a specified amount to the Consumer. 
    // If this amount does not fully meet the requested amount, that amount will be lessened by the contracted amount, 
    // and the Hub will continue to try and find a Provider for the remainder.
    public class ProductTradingHub
    {
        Dictionary<string, List<ProductRequest>> _consumerRequests;
        Dictionary<string, List<ProductRequest>> _providerRequests;

		// Attempts to fulfill the request, if there is not enough currently provided, it is queued
		// Spent providers are removed along the way
        public void RequestProvide(ProductRequest provider)
        {
	        if (_consumerRequests.ContainsKey(provider.ProductName))
	        {
		        // iterate the list of consumers for a given product
		        foreach (var consumer in _consumerRequests[provider.ProductName])
		        {
			        // if the consumer can complete the request, do it and break
			        if (consumer.Amount > provider.Amount)
			        {
				        consumer.Amount -= provider.Amount;
				        provider.Amount = 0;
				        break;
			        }

			        // otherwise, deplete its remaining amount and move to the next
			        provider.Amount -= consumer.Amount;
			        consumer.Amount = 0;
		        }

		        // remove that type if it is not empty
		        _consumerRequests[provider.ProductName].RemoveAll(i => i.Amount == 0);
		        if (!_consumerRequests[provider.ProductName].Any())
			        _consumerRequests.Remove(provider.ProductName);
	        }


	        if (provider.Amount == 0)
				return;

			// if consumer is not fulfilled, add to the backlog, creating a list if there isn't one
			if (!_consumerRequests.ContainsKey(provider.ProductName))
				_consumerRequests.Add(provider.ProductName, new List<ProductRequest>());

			_consumerRequests[provider.ProductName].Add(provider);
		}

		public void RequestConsume(ProductRequest consumer)
		{
			if (_providerRequests.ContainsKey(consumer.ProductName))
			{
				// iterate the list of providers for a given product
				foreach (var provider in _providerRequests[consumer.ProductName])
				{
					// if the provider can complete the request, do it and break
					if (provider.Amount > consumer.Amount)
					{
						provider.Amount -= consumer.Amount;
						consumer.Amount = 0;
						break;
					}

					// otherwise, deplete the remaining provider and move to the next
					consumer.Amount -= provider.Amount;
					provider.Amount = 0;
				}

				// remove that type if it is not empty
				_providerRequests[consumer.ProductName].RemoveAll(i => i.Amount == 0);
				if (!_providerRequests[consumer.ProductName].Any())
					_providerRequests.Remove(consumer.ProductName);
			}

			if (consumer.Amount == 0)
				return;

			// if consumer is not fulfilled, add to the backlog, creating a list if there isn't one
			if (!_consumerRequests.ContainsKey(consumer.ProductName))
				_consumerRequests.Add(consumer.ProductName, new List<ProductRequest>());

			_consumerRequests[consumer.ProductName].Add(consumer);
		}
	}

    // An actor in the trading system
    // This object should eventually have information about 
    // location, diplomatic faction, whatever the hub can use to decide if its a fit
    // OR, does the hub ask the trader if he will willingly trade with the other?
    public class ProductTrader
    {
        public string Name;
	    public int Budget;
    }

    // One trade consideration could be Distance. Distance can work such that 
    // if the distance is too far, the traders can buy fuel along the way (from us!)
    // Good/easy early game income, maybe a small production chain to produce it
    // Great material for a tutorial (you explored a planet nearby that has X! 
    // You are already producing Y in your base, build a factory to combine them to make fuel.)


    // ProductTraders will submit these requests to the hub when they either 
    // need goods (Consume), or they have excess good they want to trade (Provide)
    public class ProductRequest
    {
        public string ProductName;
        public int Amount;
	    public ProductTrader Trader;
    }
}