using Assets.Logistics.Ships;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using QGame;
using UnityEngine;

namespace Assets.Void
{
	public class VoidActor : QScript, ITraderDriver, IPopulationHost
	{
		private IWorldClock _worldClock;
		private ProductLookup _productLookup;

		[SerializeField] private ProductValueLookup _valueLookup;
		[SerializeField] private TraderRequestsSO _tradeRequests;
		[SerializeField] private ShipHolder _shipHolder;
		
		private const string _clientName = "Void";
		private ProductTradeAutomater _automater;
		private ProductTrader _trader;

		void Start()
		{
			_valueLookup = ProductValueLookup.Instance;
		    _worldClock = Locator.WorldClock;
		    _productLookup = ProductLookup.Instance;
			InstantiateVoidTrader();
//			Locator.MessageHub.QueueMessage(LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
		}

		private void InstantiateVoidTrader()
		{
			var go = new GameObject();
			go.transform.SetParent(transform);
			go.name = "void_trader";
			_trader = go.AddComponent<ProductTrader>();
			_trader.Initialize(this, _clientName);
			
			_automater = go.AddComponent<ProductTradeAutomater>();
			_automater.Initialize(_trader, Locator.WorldClock, _tradeRequests);
		}

		public void HandleProvideSuccess(TradeManifest manifest)
		{
			
		}

//		public string ClientName { get { return _clientName; } }
//		public void OnTransitArrival(TransitControl.Entry entry)
//		{
//			_shipHolder.BeginHold(entry.Ship);
//		}
//
//		public void OnTransitDeparture(TransitControl.Entry entry)
//		{
//			
//		}
//
//		public void HandleCargoRequested(CargoManifest manifest)
//		{
//			var ships = _shipHolder.ShipsOnHold.Where(i => i.CanTakeCargo(manifest));
//			
//		}
//
//		public void Resume(Ship ship)
//		{
//			_shipHolder.BeginHold(ship, true);
//		}

		public bool IsSimpleHold { get { return true; } }

		public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided) { return true; }

		public bool WillProvideTo(ProductTrader consumer, ProductAmount provided) {  return true; }

		public void HandleConsumeSuccess(TradeManifest manifest) { }
		public float CurrentQualityOfLife { get; private set; }
		public bool PopulationWillMigrateTo(IPopulationHost otherHost)
		{
			return true;
		}
	}
}