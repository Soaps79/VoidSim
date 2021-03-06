﻿using System;
using System.Linq;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using NUnit.Framework;
using QGame;
using UnityEngine;

namespace Assets.Editor
{
	public class TestLastIdManager : ILastIdManager
	{
		private int _index = 0;

		public int GetNext(string idName)
		{
			_index++;
			return _index;
		}

		public void Reset(string idName) { }
	}

	public class ProductTradingHubTests : ProductTradingHub
	{
		private int _index;

		[OneTimeSetUp]
		public void InitialSetup()
		{
			ServiceLocator.Register<ILastIdManager>(new TestLastIdManager());
		}

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
			var trader = go.AddComponent<ProductTrader>();
			trader.Initialize(null, "Test" + _index);

			if (isProviding)
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
			provider.OnProvideMatch += i => { provided = true; };

			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, AMOUNT);
			consumer.OnConsumeMatch += i => { consumed = true; };

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
			provider.OnProvideMatch += i => { provided = true; };

			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer.OnConsumeMatch += i => { consumed = true; };

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
			provider.OnProvideMatch += i => { provided = true; };

			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer.OnConsumeMatch += i => { consumed = true; };

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
			provider.OnProvideMatch += i => { provided++; };

			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer.OnConsumeMatch += i => { consumed++; };

			var consumer2 = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer2.OnConsumeMatch += i => { consumed++; };

			var consumer3 = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer3.OnConsumeMatch += i => { consumed++; };

			CheckForTrades();

			Assert.AreEqual(3, provided);
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
			provider.OnProvideMatch += i => { provided++; };

			var provider2 = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
			provider2.OnProvideMatch += i => { provided++; };

			var provider3 = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
			provider3.OnProvideMatch += i => { provided++; };

			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			consumer.OnConsumeMatch += i => { consumed++; };

			CheckForTrades();

			Assert.AreEqual(3, provided);
			Assert.AreEqual(3, consumed);
			Assert.IsTrue(!consumer.Consuming.Any());
		}

		[Test]
		public void ParentlessTraderStillTrades()
		{
			const int PRODUCT_ID = 1;
			const int PROVIDE_AMOUNT = 10;
			const int CONSUME_AMOUNT = 30;

			var provider = GenerateAndAddTrader(true, PRODUCT_ID, PROVIDE_AMOUNT);
			var consumer = GenerateAndAddTrader(false, PRODUCT_ID, CONSUME_AMOUNT);
			var provided = new ProductAmount{ ProductId = PRODUCT_ID, Amount = CONSUME_AMOUNT };


			CheckForTrades();

			Assert.IsTrue(provider.WillProvideTo(consumer, provided));
			Assert.IsTrue(provider.WillConsumeFrom(provider, provided));
		}
	}
}