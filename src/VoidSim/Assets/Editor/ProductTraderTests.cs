using System.Linq;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class ProductTraderTests
    {
        [Test]
        public void SetConsume_ZeroStopsConsuming()
        {
            const int productId = 8;
            const int amount = 10;
            var go = new GameObject();
            var trader = go.AddComponent<ProductTrader>();
            trader.Initialize(null, "Test");

            trader.SetConsume(new ProductAmount{ ProductId = productId, Amount = amount});

            Assert.AreEqual(1, trader.Consuming.Count);
            Assert.AreEqual(amount, trader.Consuming.First().Amount);

            trader.SetConsume(new ProductAmount { ProductId = productId, Amount = 0 });
            Assert.IsFalse(trader.Consuming.Any());
        }

        [Test]
        public void SetProvide_ZeroStopsConsuming()
        {
            const int productId = 8;
            const int amount = 10;
            var go = new GameObject();
            var trader = go.AddComponent<ProductTrader>();
            trader.Initialize(null, "Test");

            trader.SetProvide(new ProductAmount { ProductId = productId, Amount = amount });

            Assert.AreEqual(1, trader.Providing.Count);
            Assert.AreEqual(amount, trader.Providing.First().Amount);

            trader.SetProvide(new ProductAmount { ProductId = productId, Amount = 0 });
            Assert.IsFalse(trader.Providing.Any());
        }
    }
}