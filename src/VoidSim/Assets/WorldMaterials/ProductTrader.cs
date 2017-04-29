using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using QGame;

namespace Assets.WorldMaterials
{
    /// <summary>
    /// Actors will use one of these objects to manage their participation in the galaxy's trading system
    /// </summary>
    public class ProductTrader : QScript
    {
        public const string MessageName = "TraderInstance";

        public readonly List<ProductAmount> Providing = new List<ProductAmount>();
        public readonly List<ProductAmount> Consuming = new List<ProductAmount>();

        public Action<int, int, ProductTrader> OnProvideSuccess;
        public Action<int, int, ProductTrader> OnConsumeSucess;

        public void HandleProvideSuccess(int productId, int amount, ProductTrader consumer)
        {
            if (OnProvideSuccess != null)
                OnProvideSuccess(productId, amount, consumer);
        }

        public void HandleConsumeSuccess(int productId, int amount, ProductTrader provider)
        {
            if (OnConsumeSucess != null)
                OnConsumeSucess(productId, amount, provider);
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