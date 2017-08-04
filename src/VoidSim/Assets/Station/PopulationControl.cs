using System.Collections.Generic;
using System.Linq;
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
    public class PopulationControl : QScript, IPopulationHost, IProductTraderDriver, IMessageListener
    {
        [SerializeField] private int _totalCapacity;
        [SerializeField] private int _initialCapacity;
        private readonly List<PopHousing> _housing = new List<PopHousing>();
        private const string POPULATION_PRODUCT_NAME = "Population";
        private Inventory _inventory;
        private int _populationProductId;
	    private ProductTrader _trader;

        public void Initialize(Inventory inventory, int initialCapacity = 0)
        {
            _initialCapacity = initialCapacity;
            _inventory = inventory;
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

	    private void InitializeProductTrader()
	    {
		    
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
            _inventory.SetProductMaxAmount(_populationProductId, _totalCapacity);
        }

        private void UpdateCapacity()
        {
            _totalCapacity = _initialCapacity + _housing.Sum(i => i.Capacity);

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
		    return true;
	    }

	    public bool WillProvideTo(ProductTrader consumer, ProductAmount provided)
	    {
			return true;
		}

	    public void HandleProvideSuccess(TradeManifest manifest)
	    {
		    
	    }

	    public void HandleConsumeSuccess(TradeManifest manifest)
	    {
		    
	    }
    }
}