using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Ships;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
using Zenject;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Void
{
	public class VoidActor : QScript, ITransitLocation, IProductTraderDriver
	{
		[Inject] private WorldClock _worldClock;
		[Inject] private ProductLookup _productLookup;

		[SerializeField] private ProductValueLookup _valueLookup;
		[SerializeField] private TraderRequestsSO _tradeRequests;
		[SerializeField] private ShipHolder _holder;
		
		private const string _clientName = "Void";
		private ProductTradeAutomater _automater;
		private ProductTrader _trader;

		void Start()
		{
			_valueLookup = ProductValueLookup.Instance;
			InstantiateVoidTrader();
			Locator.MessageHub.QueueMessage(TradeMessages.TraderCreated, new TraderInstanceMessageArgs { Trader = _trader });
			Locator.MessageHub.QueueMessage(LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
		}

		private void InstantiateVoidTrader()
		{
			var go = new GameObject();
			go.transform.SetParent(transform);
			go.name = "void_trader";
			_trader = go.AddComponent<ProductTrader>();
			_trader.ClientName = ClientName;
			_trader.Initialize(this);
			Locator.MessageHub.QueueMessage(TradeMessages.TraderCreated, new TraderInstanceMessageArgs { Trader = _trader });

			_automater = go.AddComponent<ProductTradeAutomater>();
			_automater.Initialize(_trader, _worldClock, _tradeRequests);
		}

		public void HandleProvideSuccess(TradeManifest manifest)
		{
			// request cargo for trade
			Locator.MessageHub.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
			{
				Manifest = new CargoManifest(manifest)
				{
					Seller = ClientName,
					Buyer = manifest.Consumer,
					Currency = _valueLookup.GetValueOfProductAmount(manifest.ProductId, manifest.AmountTotal),
					ProductAmount = new ProductAmount { ProductId = manifest.ProductId, Amount = manifest.AmountTotal }
				}
			});
		}

		public string ClientName { get { return _clientName; } }
		public void OnTransitArrival(TransitControl.Entry entry)
		{
			_holder.BeginHold(entry.Ship);
		}

		public void OnTransitDeparture(TransitControl.Entry entry)
		{
			
		}

		public void Resume(Ship ship)
		{
			_holder.BeginHold(ship, true);
		}

		public bool IsSimpleHold { get { return true; } }

		public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided) { return true; }

		public bool WillProvideTo(ProductTrader consumer, ProductAmount provided) { return true; }

		public void HandleConsumeSuccess(TradeManifest manifest) { }
	}
}