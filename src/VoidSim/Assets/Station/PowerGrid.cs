using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables;
using Assets.Placeables.Nodes;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;

using UnityEngine;
using Messaging;
using QGame;

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
            MessageHub.Instance.AddListener(this, EnergyConsumer.MessageName);
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
        }

        private void TickEnergyCosts(object sender, EventArgs e)
        {
            if (HasShortage)
            {
                if (_inventory.HasProduct(_energyProduct.ID, (int) _currentTotalDemand))
                {
                    HasShortage = false;
                }
            }

            foreach (var consumer in _consumers)
            {
                if (!_inventory.TryRemoveProduct(ENERGY_PRODUCT_NAME, (int) consumer.TotalAmountConsumed))
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

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type != EnergyConsumer.MessageName)
                return;

            var placed = args as EnergyConsumerMessageArgs;
            if (placed == null) return;

            var consumer = placed.EnergyConsumer;
            if (consumer != null)
                AddConsumer(consumer);
        }

        public string Name
        {
            get { return "PowerGrid"; }
        }
    }
}
