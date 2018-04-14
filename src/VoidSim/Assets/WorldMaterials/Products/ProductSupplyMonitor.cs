using System;
using Assets.Scripts;
using Messaging;

namespace Assets.WorldMaterials.Products
{
    public class ProductSupplyMonitorCreatedMessageArgs : MessageArgs
    {
        public ProductSupplyMonitor SupplyMonitor;
    }

    public enum ProductSupplyDisplayMode
    {
        Difference, OutOfMax, SupplyOnly
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
            public StationInventory StationInventory;
            public TimeUnit SupplyUpdatefrequency;
            public TimeUnit ChangeUpdateFrequency;
            public ProductSupplyDisplayMode Mode;
            public string DisplayName;
        }

        public const string CreatedMessageType = "ProductSupplyMonitor created";

        public Product Product { get; private set; }
        private StationInventory _stationInventory;
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
            Product = data.Product;
            _stationInventory = data.StationInventory;
            _supplyUpdatefrequency = data.SupplyUpdatefrequency;
            _changeUpdateFrequency = data.ChangeUpdateFrequency;

            SetDisplayName(string.IsNullOrEmpty(data.DisplayName) ? Product.Name : data.DisplayName);
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
                case ProductSupplyDisplayMode.SupplyOnly:
                    _getAmountOutput = GetAmountSupplyOnly;
                    break;
                default:

                    throw new ArgumentOutOfRangeException("ProductSupplyDisplayMode", mode, null);
            }
        }

        private void BindToWorldClock()
        {
            Locator.WorldClock.RegisterCallback(_supplyUpdatefrequency, UpdateSupply);
            Locator.WorldClock.RegisterCallback(_changeUpdateFrequency, UpdateChange);
        }

        private void UpdateChange(object sender, EventArgs e)
        {
            _currentChange = _currentSupply - _lastSupply;
            _lastSupply = _currentSupply;
        }

        private void UpdateSupply(object sender, EventArgs e)
        {
            _currentSupply = _stationInventory.Products.GetProductCurrentAmount(Product.ID);
            _currentMax = _stationInventory.Products.GetProductMaxAmount(Product.ID);
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

        private string GetAmountSupplyOnly()
        {
            return _currentSupply.ToString();
        }

    }
}
