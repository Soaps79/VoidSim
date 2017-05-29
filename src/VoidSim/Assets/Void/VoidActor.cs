using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Ships;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
using Zenject;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Void
{
	public class VoidActor : QScript, ITransitLocation
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
			MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });
			MessageHub.Instance.QueueMessage(LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
		}

		private void InstantiateVoidTrader()
		{
			var go = new GameObject();
			go.transform.SetParent(transform);
			go.name = "void_trader";
			_trader = go.AddComponent<ProductTrader>();
			_trader.ClientName = ClientName;
			_trader.OnProvideMatch += HandleProvideMatch;
			MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

			_automater = go.AddComponent<ProductTradeAutomater>();
			_automater.Initialize(_trader, _worldClock, _tradeRequests);
		}

		private void HandleProvideMatch(TradeInfo info)
		{
			// request cargo for trade
			MessageHub.Instance.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
			{
				Manifest = new CargoManifest(info)
				{
					Seller = ClientName,
					Buyer = info.Consumer.ClientName,
					Currency = _valueLookup.GetValueOfProductAmount(info.ProductId, info.AmountTotal),
					ProductAmount = new ProductAmount { ProductId = info.ProductId, Amount = info.AmountTotal }
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

		public bool IsSimpleHold { get { return true; } }
	}
}