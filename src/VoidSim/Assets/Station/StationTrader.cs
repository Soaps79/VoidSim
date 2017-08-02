using Assets.Logistics;
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
//	bool WillConsumeFrom(ProductTrader provider, ProductAmount provided);
//bool WillProvideTo(ProductTrader consumer, ProductAmount provided);
//void HandleProvideSuccess(TradeManifest manifest);
//void HandleConsumeSuccess(TradeManifest manifest);

	/// <summary>
	/// This is currently doing multiple small jobs that will most likely become their own behaviors or systems.
	/// Acts as a driver for ProductTrader, telling it what to buy and sell on the market. 
	/// Ties an Inventory to a ProductTrader, moving products in and out as transactions complete.
	/// Handles broadcasting of successful trades. Currently works only because Station is one of 2 actors, involved in every trade.
	///     As Void develops, trades between other actors will not be broadcast. Not sure if will be relevant.
	///     This seems like a job that should be handled by TradingHub, but then it has to know about currency, so ???
	/// </summary>
	public class StationTrader : QScript, IProductTraderDriver, ISerializeData<InventoryReserveData>
    {
        private InventoryReserve _reserve;
        public string ClientName { get; set; }

        [SerializeField] private ProductValueLookup _valueLookup;
        [SerializeField] private Inventory _inventory;
        private ProductTrader _trader;
        private WorldClock _worldClock;

	    private readonly CollectionSerializer<InventoryReserveData> _serializer
		    = new CollectionSerializer<InventoryReserveData>();

		public void Initialize(Inventory inventory, InventoryReserve reserve)
        {
            _inventory = inventory;
            _inventory.OnInventoryChanged += CheckForTrade;
            _reserve = reserve;
	        _reserve.OnReserveChanged += CheckForTrade;

            BindToTrader();
            _valueLookup = ProductValueLookup.Instance;

	        if (_serializer.HasDataFor(this, "StationTrader"))
		        HandleGameLoad();

			Locator.MessageHub.QueueMessage(TradeMessages.TraderCreated, new TraderInstanceMessageArgs { Trader = _trader });
        }

	    private void BindToTrader()
        {
            _trader = gameObject.AddComponent<ProductTrader>();
            _trader.ClientName = ClientName;
			_trader.Initialize(this);
        }

		private void HandleGameLoad()
		{
			var data = _serializer.DeserializeData();
			_reserve.SetFromData(data);
		}

	    public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided) { return true; }

	    public bool WillProvideTo(ProductTrader consumer, ProductAmount provided) { return true; }

	    public void HandleProvideSuccess(TradeManifest manifest)
        {
            // need to add logic to place a hold on the traded items in the reserve
            _reserve.AdjustHold(manifest.ProductId, -manifest.AmountTotal);

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

            CheckForTrade();
        }

        public void HandleConsumeSuccess(TradeManifest manifest)
        {
			_reserve.AdjustHold(manifest.ProductId, manifest.AmountTotal);
			CheckForTrade();
        }

        private void CheckForTrade()
        {
            var list = _reserve.GetProvideProducts();
            list.ForEach(i => _trader.SetProvide(i));

            list = _reserve.GetConsumeProducts();
            list.ForEach(i => _trader.SetConsume(i));
        }

	    public InventoryReserveData GetData()
	    {
		    return _reserve.GetData();
	    }
    }
}