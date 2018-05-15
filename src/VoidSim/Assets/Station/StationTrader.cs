using Assets.Logistics;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Station
{
	/// <summary>
	/// This is currently doing multiple small jobs that will most likely become their own behaviors or systems.
	/// Acts as a driver for ProductTrader, telling it what to buy and sell on the market. 
	/// Ties an Inventory to a ProductTrader, moving products in and out as transactions complete.
	/// Handles broadcasting of successful trades. Currently works only because Station is one of 2 actors, involved in every trade.
	///     As Void develops, trades between other actors will not be broadcast. Not sure if will be relevant.
	///     This seems like a job that should be handled by TradingHub, but then it has to know about currency, so ???
	/// </summary>
	public class StationTrader : QScript, ITraderDriver, ISerializeData<InventoryReserveData>
    {
        private InventoryReserve _reserve;

        [SerializeField] private ProductValueLookup _valueLookup;
        [SerializeField] private ProductInventory _stationInventory;
        private ProductTrader _trader;
        private WorldClock _worldClock;

	    private readonly CollectionSerializer<InventoryReserveData> _serializer
		    = new CollectionSerializer<InventoryReserveData>();

	    public void Initialize(ProductInventory stationInventory, InventoryReserve reserve)
        {
            _stationInventory = stationInventory;
            _stationInventory.OnProductsChanged += HandleProductsChanged;
            _reserve = reserve;
	        _reserve.OnReserveChanged += CheckForTrade;

            BindToTrader();
            _valueLookup = ProductValueLookup.Instance;

	        if (_serializer.HasDataFor(this, "StationTrader"))
		        HandleGameLoad();
        }

        private void BindToTrader()
        {
            _trader = gameObject.AddComponent<ProductTrader>();
			_trader.Initialize(this, Station.ClientName, true);
        }

		private void HandleGameLoad()
		{
			var data = _serializer.DeserializeData();
			_reserve.SetFromData(data);
		}

	    public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided)
	    {
			//if(ProductLookup.Instance.GetProduct(provided.ProductId).Name == ProductNameLookup.Population)
			//	return _popControl.WillConsumeFrom()
			return true;
	    }

	    public bool WillProvideTo(ProductTrader consumer, ProductAmount provided) { return true; }

	    public void HandleProvideSuccess(TradeManifest manifest)
        {
            _reserve.AdjustHold(manifest.ProductId, -manifest.AmountTotal);
            CheckForTrade();
        }

        public void HandleConsumeSuccess(TradeManifest manifest)
        {
			_reserve.AdjustHold(manifest.ProductId, manifest.AmountTotal);
			CheckForTrade();
        }

        private void HandleProductsChanged(int productId, int amount)
        {
            CheckForTrade();
        }

        private void CheckForTrade()
        {
            var list = _reserve.GetProvideProducts();
            list.ForEach(i => _trader.SetProvide(i));

            list = _reserve.GetConsumeProducts();
            list.ForEach(i => _trader.SetConsume(i));
        }

	    public void SetProvide(ProductAmount productAmount)
	    {
		    _trader.SetProvide(productAmount);
	    }

	    public void SetConsume(ProductAmount productAmount)
	    {
		    _trader.SetConsume(productAmount);
	    }

	    public InventoryReserveData GetData()
	    {
		    return _reserve.GetData();
	    }
    }
}