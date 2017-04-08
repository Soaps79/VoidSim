using System;
using Assets.Scripts.WorldMaterials;
using Messaging;
using UnityEngine;

namespace Assets.WorldMaterials
{
    public class ProductSupplyMonitorCreatedMessageArgs : MessageArgs
    {
        public ProductSupplyMonitor SupplyMonitor;
    }

    /// <summary>
    /// Simple class that monitors an inventory, and at a regular interval records the 
    /// current supply of a given product, and the difference from the last update
    /// </summary>
    public class ProductSupplyMonitor
    {
        public const string CreatedMessageType = "ProductSupplyMonitor created";

        private Product _product;
        private Inventory _inventory;
        private TimeUnit _supplyUpdatefrequency;
        private TimeUnit _changeUpdateFrequency;

        private int _lastSupply;
        private int _currentSupply;
        private int _currentChange;
        public string DisplayName;

        public ProductSupplyMonitor(Product product, Inventory inventory, TimeUnit supplyUpdatefrequency, TimeUnit changeUpdateFrequency)
        {
            _product = product;
            _inventory = inventory;
            _supplyUpdatefrequency = supplyUpdatefrequency;
            _changeUpdateFrequency = changeUpdateFrequency;
            SetDisplayName(_product.Name);

            BindToWorldClock();
            UpdateSupply(null, null);
            UpdateChange(null, null);
            _currentChange = 0;
        }

        /// <summary>
        /// Display name will be product name by default, customize it here
        /// </summary>
        public void SetDisplayName(string displayName)
        {
            DisplayName = string.Format("{0}:", displayName);
        }

        private void BindToWorldClock()
        {
            WorldClock.Instance.RegisterCallback(_supplyUpdatefrequency, UpdateSupply);
            WorldClock.Instance.RegisterCallback(_changeUpdateFrequency, UpdateChange);
        }

        private void UpdateChange(object sender, EventArgs e)
        {
            _currentChange = _currentSupply - _lastSupply;
            _lastSupply = _currentSupply;
        }

        private void UpdateSupply(object sender, EventArgs e)
        {
            _currentSupply = _inventory.GetProductCurrentAmount(_product.ID);
        }

        public string GetAmountOutput()
        {
            return string.Format("{0} ({1})", _currentSupply, _currentChange);
        }
    }
}
