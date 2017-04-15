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

    public enum ProductSupplyDisplayMode
    {
        Difference, OutOfMax
    }

    /// <summary>
    /// Simple class that monitors an inventory, and at a regular interval records the 
    /// current supply of a given product, and the difference from the last update
    /// </summary>
    public class ProductSupplyMonitor
    {
        public class Data
        {
            public Product Product;
            public Inventory Inventory;
            public TimeUnit SupplyUpdatefrequency;
            public TimeUnit ChangeUpdateFrequency;
            public ProductSupplyDisplayMode Mode;
            public string DisplayName;
        }

        public const string CreatedMessageType = "ProductSupplyMonitor created";

        private Product _product;
        private Inventory _inventory;
        private TimeUnit _supplyUpdatefrequency;
        private TimeUnit _changeUpdateFrequency;

        private int _lastSupply;
        private int _currentSupply;
        private int _currentChange;
        private int _currentMax;
        public string DisplayName;

        private Func<string> _getAmountOutput;

        public ProductSupplyMonitor(Data data)
        {
            _product = data.Product;
            _inventory = data.Inventory;
            _supplyUpdatefrequency = data.SupplyUpdatefrequency;
            _changeUpdateFrequency = data.ChangeUpdateFrequency;

            SetDisplayName(string.IsNullOrEmpty(data.DisplayName) ? _product.Name : data.DisplayName);
            SetDisplayMode(data.Mode);

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

        public void SetDisplayMode(ProductSupplyDisplayMode mode)
        {
            switch (mode)
            {
                case ProductSupplyDisplayMode.Difference:
                    _getAmountOutput = GetAmountDifference;
                    break;
                case ProductSupplyDisplayMode.OutOfMax:
                    _getAmountOutput = GetAmountOutOfMax;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ProductSupplyDisplayMode", mode, null);
            }
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
            _currentMax = _inventory.GetProductMaxAmount(_product.ID);
        }

        public string GetAmountOutput()
        {
            return _getAmountOutput();
        }

        private string GetAmountDifference()
        {
            return string.Format("{0} ({1})", _currentSupply, _currentChange);
        }

        private string GetAmountOutOfMax()
        {
            return string.Format("{0} ({1})", _currentSupply, _currentMax);
        }
    }
}
