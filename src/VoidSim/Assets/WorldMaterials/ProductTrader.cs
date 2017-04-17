using System;
using System.Collections.Generic;
using QGame;

namespace Assets.WorldMaterials
{
    /// <summary>
    /// Actors will use one of these objects to manage their participation in the galaxy's trading system
    /// </summary>
    public class ProductTrader : QScript
    {
        public const string MessageName = "TraderInstance";

        public readonly List<ProductTradeRequest> Providing = new List<ProductTradeRequest>();
        public readonly List<ProductTradeRequest> Consuming = new List<ProductTradeRequest>();

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
    }
}