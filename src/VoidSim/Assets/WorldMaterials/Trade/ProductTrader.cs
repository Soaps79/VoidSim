using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using QGame;

namespace Assets.WorldMaterials.Trade
{
    /// <summary>
    /// Actors will use one of these objects to manage their participation in the galaxy's trading system
    /// </summary>
    public class ProductTrader : QScript
    {
        public const string MessageName = "TraderInstance";

        public readonly List<ProductAmount> Providing = new List<ProductAmount>();
        public readonly List<ProductAmount> Consuming = new List<ProductAmount>();

        public Action<TradeInfo> OnProvideMatch;
        public Action<TradeInfo> OnConsumeMatch;

        public void HandleProvideSuccess(TradeInfo info)
        {
            if (OnProvideMatch != null)
                OnProvideMatch(info);
        }

        public void HandleConsumeSuccess(TradeInfo info)
        {
            if (OnConsumeMatch != null)
                OnConsumeMatch(info);
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