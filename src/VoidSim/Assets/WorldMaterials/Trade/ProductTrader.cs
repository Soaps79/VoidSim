using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;

namespace Assets.WorldMaterials.Trade
{
	public class TraderInstanceMessageArgs : MessageArgs
	{
		public ProductTrader Trader;
	}

	/// <summary>
	/// Actors will use one of these objects to manage their participation in the galaxy's trading system
	/// </summary>
	public class ProductTrader : QScript
    {
        
        public string ClientName;

        public readonly List<ProductAmount> Providing = new List<ProductAmount>();
        public readonly List<ProductAmount> Consuming = new List<ProductAmount>();

        public Action<TradeManifest> OnProvideMatch;
        public Action<TradeManifest> OnConsumeMatch;

        public void HandleProvideSuccess(TradeManifest manifest)
        {
            if (OnProvideMatch != null)
                OnProvideMatch(manifest);
        }

        public void HandleConsumeSuccess(TradeManifest manifest)
        {
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
        }
    }
}