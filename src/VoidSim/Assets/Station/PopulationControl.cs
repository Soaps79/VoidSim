using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    /// <summary>
    /// Placeholder for where pop will be managed
    /// </summary>
    public class PopulationControl : QScript, IPopulationHost, ITraderDriver, IMessageListener
    {
        [SerializeField] private int _totalCapacity;
        [SerializeField] private int _initialCapacity;
        private readonly List<PopHousing> _housing = new List<PopHousing>();
        private const string POPULATION_PRODUCT_NAME = "Population";
        private Inventory _inventory;
        private int _populationProductId;
		// When there is room for more population, it it requested through this Trader which is hooked into the trade system
	    private ProductTrader _trader;
	    private int _inboundPopulation;

        public void Initialize(Inventory inventory, int initialCapacity = 0)
        {
            _initialCapacity = initialCapacity;
            _inventory = inventory;
	        _inventory.OnProductsChanged += HandleInventoryProductChanged;

            var pop = ProductLookup.Instance.GetProduct(POPULATION_PRODUCT_NAME);
            _populationProductId = pop.ID;
	        CurrentQualityOfLife = 10;

            if (_initialCapacity > 0)
                _inventory.SetProductMaxAmount(_populationProductId, _initialCapacity);

			Locator.MessageHub.AddListener(this, PopHousing.MessageName);

			// remove when housing serialization is in place
			Locator.LastId.Reset("pop_housing");

	        InitializeProductTrader();
        }

	    private void HandleInventoryProductChanged(int productId, int amount)
	    {
		    if (productId == _populationProductId)
			    _inboundPopulation -= amount;
	    }

	    private void InitializeProductTrader()
	    {
		    _trader = gameObject.AddComponent<ProductTrader>();
			_trader.Initialize(this, Station.ClientName);
		    UpdateTradeRequest();
	    }

		// checks to see if inventory has room for more pop (discounting those already in transit)
	    private void UpdateTradeRequest()
	    {
		    var remaining = _inventory.GetProductRemainingSpace(_populationProductId);
		    remaining -= _inboundPopulation;
		    _trader.SetConsume(new ProductAmount
		    {
			    ProductId = _populationProductId,
				Amount = remaining > 0  ? remaining : 0
		    });
	    }

	    public int TotalCapacity
        {
            get { return _totalCapacity; }
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PopHousing.MessageName)
                HandleHousingAdd(args as PopHousingMessageArgs);
        }

        private void HandleHousingAdd(PopHousingMessageArgs args)
        {
            if (args == null || args.PopHousing == null)
            {
                Debug.Log("PopulationControl given bad consumer message args.");
                return;
            }

	        args.PopHousing.name = "pop_housing_" + Locator.LastId.GetNext("pop_housing");

            _housing.Add(args.PopHousing);
            UpdateCapacity();
			UpdateTradeRequest();
        }

        private void UpdateCapacity()
        {
            _totalCapacity = _initialCapacity + _housing.Sum(i => i.Capacity);
	        _inventory.SetProductMaxAmount(_populationProductId, _totalCapacity);
		}

		public string Name
        {
            get { return "PopulationControl"; }
        }

	    public float CurrentQualityOfLife { get; private set; }
	    public bool PopulationWillMigrateTo(IPopulationHost otherHost)
	    {
		    return false;
	    }

	    public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided)
	    {
			if(provided.ProductId != _populationProductId)
				throw new UnityException("PopulationControl offered trade that was not population");

		    _inboundPopulation += provided.Amount;
		    return true;
	    }

	    public bool WillProvideTo(ProductTrader consumer, ProductAmount provided)
	    {
			return true;
		}

	    public void HandleProvideSuccess(TradeManifest manifest)
	    {
			Locator.MessageHub.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
			{
				Manifest = new CargoManifest(manifest)
				{
					Seller = Station.ClientName,
					Buyer = manifest.Consumer,
					Currency = 0,
					ProductAmount = new ProductAmount { ProductId = _populationProductId, Amount = manifest.AmountTotal }
				}
			});
		}

	    public void HandleConsumeSuccess(TradeManifest manifest)
	    {
		    
	    }
    }
}