using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using UnityEngine;
using Messaging;
using QGame;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Station
{
    /// <summary>
    /// Handles tracking and fulfilling energy requirements.
    /// Needs to be given an Inventory, and then once per tick (currently Daily) it will remove 
    /// enough energy from it to fulfill the needs of all registered consumers. 
    /// </summary>
    public class PowerGrid : QScript, IMessageListener
    {
        public const string ENERGY_PRODUCT_NAME = "Energy";

        private Inventory _inventory;
        private WorldClock _worldClock;
        private readonly List<EnergyConsumer> _consumers = new List<EnergyConsumer>();
        private Product _energyProduct;

        [SerializeField] private float _currentTotalDemand;
        [SerializeField] private float _currentTotalProvided;
        public bool HasShortage { get; private set; }
        public float TotalDemand { get { return _currentTotalDemand; } }

        public Action<PowerGrid> OnTotalConsumptionChanged;

        void Start ()
        {
            // learns of new consumers through messages
            Locator.MessageHub.AddListener(this, EnergyConsumer.MessageName);
            Locator.MessageHub.AddListener(this, ProductFactory.MessageName);
        }

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
            if (_worldClock == null)
            {
                _worldClock = WorldClock.Instance;
                _worldClock.OnDayUp += TickEnergyCosts;
            }

            if (_energyProduct == null)
            {
                _energyProduct = ProductLookup.Instance.GetProduct(ENERGY_PRODUCT_NAME);
            }

			// remove this when energy serialization is in place 
	        Locator.LastId.Reset("power_plant");
        }

        // this was written quickly, might need to be made more robust later
        private void TickEnergyCosts(object sender, EventArgs e)
        {
			// TODO: new flow:
			// get current total
			// find percentage that can currently be filled
			// tick all amounts from inventory
			// set consumer current efficiency if it has changed

            if (HasShortage)
            {
                if (_inventory.HasProduct(_energyProduct.ID, (int) _currentTotalDemand))
                {
                    HasShortage = false;
                }
            }

			foreach (var consumer in _consumers)
            {
                if (_inventory.TryRemoveProduct(_energyProduct.ID, (int) consumer.TotalAmountConsumed) < consumer.TotalAmountConsumed)
                    HasShortage = true;
            }
        }

        private void AddConsumer(EnergyConsumer consumer)
        {
            if (consumer == null) return;
            consumer.OnAmountConsumedChanged += HandleConsumerAmountChanged;
            _consumers.Add(consumer);
            Debug.Log("PowerGrid consumer added");
            UpdateDemand();
		}

        private void UpdateDemand()
        {
            var total = _consumers.Sum(i => i.AmountConsumed);
            if (_currentTotalDemand != total)
            {
                _currentTotalDemand = total;
                if (OnTotalConsumptionChanged != null)
                    OnTotalConsumptionChanged(this);
            }
        }

        private void HandleConsumerAmountChanged(object sender, EventArgs e)
        {
            var consumer = sender as EnergyConsumer;
            if (consumer == null) return;

            UpdateDemand();
        }

        // Grid gains consumers and providers through messaging
        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == EnergyConsumer.MessageName)
                HandleConsumerMessage(args as EnergyConsumerMessageArgs);
            
            // Providers aren't currently stored here, but as this is the energy manager,
            // it takes responsibility for initialization    
            else if (type == ProductFactory.MessageName)
                HandleProviderMessage(args as ProductFactoryMessageArgs);
        }

        // make sure the messages are good and handle the additions
        private void HandleConsumerMessage(EnergyConsumerMessageArgs args)
        {
            if (args == null || args.EnergyConsumer == null)
                Debug.Log("PowerGrid given bad consumer message args.");

            AddConsumer(args.EnergyConsumer);
        }

        private void HandleProviderMessage(ProductFactoryMessageArgs args)
        {
	        if (args == null || args.ProductFactory == null)
	        {
		        Debug.Log("PowerGrid given bad provider message args.");
		        return;
	        }

	        var factory = args.ProductFactory;

	        if (factory.InitialRecipe != ENERGY_PRODUCT_NAME)
		        return;

	        factory.name = "power_plant_" + Locator.LastId.GetNext("power_plant");
            AddProvider(args.ProductFactory);
        }

        private void AddProvider(ProductFactory factory)
        {
            factory.Initialize(_inventory, ProductLookup.Instance);
        }

        public string Name
        {
            get { return "PowerGrid"; }
        }
    }
}
