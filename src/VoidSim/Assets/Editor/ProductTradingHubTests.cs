using System;
using System.Linq;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class ProductTradingHubTests : ProductTradingHub
    {
        private int _index;

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            ClearLists();
        }

        private ProductTrader GenerateAndAddTrader(bool isProviding, int productId, int amount)
        {
            _index++;
            var go = new GameObject();
            go.name = "Test" + _index;
            var trader = go.AddComponent<ProductTrader>();
            if(isProviding)
                trader.Providing.Add(new ProductAmount { ProductId = productId, Amount = amount });
            else
                trader.Consuming.Add(new ProductAmount { ProductId = productId, Amount = amount });
            HandleTraderAdd(new TraderInstanceMessageArgs { Trader = trader });
            return trader;
        }

        [Test]
        public void Transaction_ProvideEntireAmount()
        {
            const int PRODUCT_ID = 1;
            const int AMOUNT = 20;

            var provided = false;
            var consumed = false;

            var provider = GenerateAndAddTrader(true, PRODUCT_ID, AMOUNT);
            provider.OnProvideSuccess += (i, j, k) => { provided = true; };

            var consumer = GenerateAndAddTrader(false, PRODUCT_ID, AMOUNT);
            consumer.OnConsumeSucess += (i, j, k) => { consumed = true; };

            CheckForTrades();

            Assert.IsTrue(provided);
            Assert.IsTrue(consumed);
        }

        [Test]
        public void Transaction_ConsumeLessThanProvided()
        {
            const int PRODUCT_ID = 1;
            const int PROVIDE_AMOUNT = 20;
            const int CONSUME_AMOUNT = 10;

            var provided = false;
            var consumed = false;

            var provider = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider.OnProvideSuccess += (i, j, k) => { provided = true; };

            var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer.OnConsumeSucess += (i, j, k) => { consumed = true; };

            CheckForTrades();

            Assert.IsTrue(provided);
            Assert.IsTrue(consumed);
            Assert.IsTrue(provider.Providing.First().Amount == Math.Abs(PROVIDE_AMOUNT - CONSUME_AMOUNT));
        }

        [Test]
        public void Transaction_ProvideLessThanConsumed()
        {
            const int PRODUCT_ID = 1;
            const int PROVIDE_AMOUNT = 10;
            const int CONSUME_AMOUNT = 20;

            var provided = false;
            var consumed = false;

            var provider = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider.OnProvideSuccess += (i, j, k) => { provided = true; };

            var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer.OnConsumeSucess += (i, j, k) => { consumed = true; };

            CheckForTrades();

            Assert.IsTrue(provided);
            Assert.IsTrue(consumed);
            Assert.IsTrue(consumer.Consuming.First().Amount == Math.Abs(PROVIDE_AMOUNT - CONSUME_AMOUNT));
        }

        [Test]
        public void Transaction_ProvideToMultipleConsumers()
        {
            const int PRODUCT_ID = 1;
            const int PROVIDE_AMOUNT = 30;
            const int CONSUME_AMOUNT = 10;

            var provided = 0;
            var consumed = 0;

            var provider = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider.OnProvideSuccess += (i, j, k) => { provided++; };

            var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer.OnConsumeSucess += (i, j, k) => { consumed++; };

            var consumer2 = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer2.OnConsumeSucess += (i, j, k) => { consumed++; };

            var consumer3 = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer3.OnConsumeSucess += (i, j, k) => { consumed++; };

            CheckForTrades();

            Assert.AreEqual(1, provided);
            Assert.AreEqual(3, consumed);
            Assert.IsTrue(!provider.Providing.Any());
        }
        [Test]
        public void Transaction_ConsumeFromMultipleProviders()
        {
            const int PRODUCT_ID = 1;
            const int PROVIDE_AMOUNT = 10;
            const int CONSUME_AMOUNT = 30;

            var provided = 0;
            var consumed = 0;

            var provider = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider.OnProvideSuccess += (i, j, k) => { provided++; };

            var provider2 = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider2.OnProvideSuccess += (i, j, k) => { provided++; };

            var provider3 = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
            provider3.OnProvideSuccess += (i, j, k) => { provided++; };

            var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
            consumer.OnConsumeSucess += (i, j, k) => { consumed++; };

            CheckForTrades();

            Assert.AreEqual(3, provided);
            Assert.AreEqual(3, consumed);
            Assert.IsTrue(!consumer.Consuming.Any());
        }
    }
}