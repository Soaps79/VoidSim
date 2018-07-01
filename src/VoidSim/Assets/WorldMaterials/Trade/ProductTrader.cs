using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;

namespace Assets.WorldMaterials.Trade
{
	public class TraderInstanceMessageArgs : MessageArgs
	{
		public ProductTrader Trader;
	}

	// the pattern used here could probably be rethought
	// this was written to fill some gaps in trade and open up population
	public interface ITraderDriver
	{
		bool WillConsumeFrom(ProductTrader provider, ProductAmount provided);
		bool WillProvideTo(ProductTrader consumer, ProductAmount provided);
		void HandleProvideSuccess(TradeManifest manifest);
		void HandleConsumeSuccess(TradeManifest manifest);
	}

	/// <summary>
	/// Actors will use one of these objects to manage their participation in the galaxy's trading system
	/// </summary>
	public class ProductTrader : QScript
	{
		public ITraderDriver Driver { get; private set; }

        public string ClientName { get; private set; }

        public List<ProductAmount> Providing { get; private set; }
        public List<ProductAmount> Consuming { get; private set; }

        public Action<TradeManifest> OnProvideMatch;
        public Action<TradeManifest> OnConsumeMatch;

		public void Initialize(ITraderDriver driver, string clientName)
		{
		    Providing = new List<ProductAmount>();
			Consuming = new List<ProductAmount>();
			Driver = driver;
			ClientName = clientName;
			Locator.MessageHub.QueueMessage(TradeMessages.TraderCreated, new TraderInstanceMessageArgs { Trader = this });
		}

        public void HandleProvideSuccess(TradeManifest manifest)
        {
			if (Driver != null)
				Driver.HandleProvideSuccess(manifest);

	        if (OnProvideMatch != null)
		        OnProvideMatch(manifest);
        }

	    public void HandleConsumeSuccess(TradeManifest manifest)
        {
			if (Driver != null)
				Driver.HandleConsumeSuccess(manifest);

	        if (OnConsumeMatch != null)
		        OnConsumeMatch(manifest);
		}

        public void SetProvide(ProductAmount productAmount)
        {
            var product = Providing.FirstOrDefault(i => i.ProductId == productAmount.ProductId);
            if (product == null)
            {
                Providing.Add(new ProductAmount(productAmount.ProductId, productAmount.Amount));
            }
            else
            {
                product.Amount = productAmount.Amount;
            }

            PruneOfferings();
        }

	    private void PruneOfferings()
	    {
	        Consuming.RemoveAll(i => i.Amount <= 0);
	        Providing.RemoveAll(i => i.Amount <= 0);
        }

	    public void SetConsume(ProductAmount productAmount)
        {
            var product = Consuming.FirstOrDefault(i => i.ProductId == productAmount.ProductId);
            if (product == null)
            {
                Consuming.Add(new ProductAmount(productAmount.ProductId, productAmount.Amount));
            }
            else
            {
                product.Amount = productAmount.Amount;
            }

            PruneOfferings();
        }

        public void AddConsume(ProductAmount productAmount)
	    {
		    AddConsume(productAmount.ProductId, productAmount.Amount);
	    }

		public void AddConsume(int productId, int amount)
	    {
		    var product = Consuming.FirstOrDefault(i => i.ProductId == productId);
		    if (product == null)
		    {
			    Consuming.Add(new ProductAmount(productId, amount));
		    }
		    else
		    {
			    product.Amount += amount;
		    }

	        PruneOfferings();
        }

        public void AddProvide(ProductAmount productAmount)
        {
            AddProvide(productAmount.ProductId, productAmount.Amount);
        }

        public void AddProvide(int productId, int amount)
        {
            var product = Providing.FirstOrDefault(i => i.ProductId == productId);
            if (product == null)
            {
                Providing.Add(new ProductAmount(productId, amount));
            }
            else
            {
                product.Amount += amount;
            }

            PruneOfferings();
        }

		// will always be true if driver is not set
	    public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided)
	    {
		    return Driver == null || Driver.WillConsumeFrom(provider, provided);
	    }

		// will always be true if driver is not set
		public bool WillProvideTo(ProductTrader consumer, ProductAmount provided)
	    {
			return Driver == null || Driver.WillProvideTo(consumer, provided);
		}

	    public Action<TradeManifest> OnResume;

	    public void HandleResume(TradeManifest manifest)
	    {
	        if (OnResume != null)
	            OnResume(manifest);
	    }
	}
}