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

namespace Assets.Void
{
	public class VoidActor : QScript, ITransitLocation
	{
		[Inject] private WorldClock _worldClock;
		[Inject] private ProductLookup _productLookup;

		[SerializeField] private ProductValueLookup _valueLookup;
		[SerializeField] private TraderRequestsSO _tradeRequests;
		[SerializeField] private TimeLength _shipDelayTime;
		private float _shipDelaySeconds;
		private const string _clientName = "Void";
		private ProductTradeAutomater _automater;
		private ProductTrader _trader;

		private readonly List<Ship> _shipsOnHold = new List<Ship>();
		private readonly List<Ship> _shipstoRemove = new List<Ship>();

		void Start()
		{
			_valueLookup = ProductValueLookup.Instance;
			_shipDelaySeconds = WorldClock.Instance.GetSeconds(_shipDelayTime);
			InstantiateVoidTrader();
			MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });
			MessageHub.Instance.QueueMessage(LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
			OnEveryUpdate += UpdateShips;
		}

		private void UpdateShips(float delta)
		{
			if (!_shipsOnHold.Any())
				return;

			foreach (var ship in _shipsOnHold)
			{
				ship.Ticker.ElapsedTicks += delta;
				if(ship.Ticker.IsComplete)
					_shipstoRemove.Add(ship);
			}

			if (!_shipstoRemove.Any())
				return;

			_shipsOnHold.RemoveAll(i => _shipstoRemove.Contains(i));
			_shipstoRemove.ForEach(i => i.CompleteTraffic());
			_shipstoRemove.Clear();
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
			// called twice
			// something to do with actor


			entry.Ship.Ticker.Reset(_shipDelaySeconds);
			entry.Ship.BeginHold(null, null);
			_shipsOnHold.Add(entry.Ship);
		}

		public void OnTransitDeparture(TransitControl.Entry entry)
		{
			
		}

		public bool IsSimpleHold { get { return true; } }
	}
}