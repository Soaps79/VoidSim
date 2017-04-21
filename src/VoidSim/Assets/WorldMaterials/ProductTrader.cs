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

        public Action<int, int> OnProvideSuccess;
        public Action<int, int> OnConsumeSucess;

        public void HandleProvideSuccess(int productId, int amount)
        {
            if (OnProvideSuccess != null)
                OnProvideSuccess(productId, amount);
        }

        public void HandleConsumeSuccess(int productId, int amount)
        {
            if (OnConsumeSucess != null)
                OnConsumeSucess(productId, amount);
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