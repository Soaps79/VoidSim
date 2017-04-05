using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Station.UI;
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
        private readonly List<EnergyConsumerNode> _consumers = new List<EnergyConsumerNode>();
        private float _currentTotalDemand;
        private float _lastTotalProvided;
        private float _currentTotalProvided;
        public bool HasShortage { get; private set; }
        public float TotalDemand { get { return _currentTotalDemand; } }

        public Action<PowerGrid> OnTotalConsumptionChanged;

        void Start ()
        {
            // learns of new consumers through messages
            MessageHub.Instance.AddListener(this, PlaceableMessages.PlaceablePlacedMessageName);
        }

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += CheckForEnergyChange;
            if (_worldClock == null)
            {
                _worldClock = WorldClock.Instance;
                _worldClock.OnDayUp += TickEnergyCosts;
            }

            MessageHub.Instance.QueueMessage(StatsMessages.StatProviderCreated, 
                new StatProviderCreatedMessageArgs
            {
                ValueProvider = new StatProvider
                {
                    Name = "Energy: ",
                    Value = GenerateCurrentPowerDisplayValue
                }
            });
        }

        private string GenerateCurrentPowerDisplayValue()
        {
            return string.Format("{0} ({1})",
                _inventory.GetProductCurrentAmount(ENERGY_PRODUCT_NAME),
                    _lastTotalProvided - _currentTotalDemand);
        }

        private void CheckForEnergyChange(string productName, int amount)
        {
            if (productName == ENERGY_PRODUCT_NAME)
                _currentTotalProvided += amount;
        }

        private void TickEnergyCosts(object sender, EventArgs e)
        {
            _lastTotalProvided = _currentTotalProvided;
            _currentTotalProvided = 0;

            if (HasShortage)
            {
                if (_inventory.HasProduct(ENERGY_PRODUCT_NAME, (int) _currentTotalDemand))
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

        private void AddConsumer(EnergyConsumerNode consumer)
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
            var consumer = sender as EnergyConsumerNode;
            if (consumer == null) return;

            UpdateDemand();
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type != PlaceableMessages.PlaceablePlacedMessageName)
                return;

            var placed = args as PlaceablePlacedArgs;
            if (placed == null) return;

            var consumer = placed.ObjectPlaced as IEnergyConsumer;
            if (consumer != null)
                AddConsumer(consumer.EnergyConsumerNode);
        }

        public string Name
        {
            get { return "PowerGrid"; }
        }
    }
}
