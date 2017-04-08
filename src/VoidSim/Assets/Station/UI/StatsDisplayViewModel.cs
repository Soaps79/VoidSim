using System.Collections.Generic;
using Assets.WorldMaterials;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station.UI
{
    /// <summary>
    /// Feeds a UI panel that shows information that is constantly relevant to the player.
    /// Listens for ProductSupplyMonitor created messages, and adds their output to the UI display
    /// Logic will need to be written so that the panel itself resizes to accomodate additional values.
    /// Further, the providers should provide their icon instead of string DisplayName.
    /// 
    /// Possible future usage would be for the player to add and remove products to the display as they come and go in relevency.
    /// </summary>
    public class StatsDisplayViewModel : QScript, IMessageListener
    {
        // acts as a binding between UI elements and inventory
        private class SupplyMonitorEntry
        {
            public Text Text;
            public ProductSupplyMonitor Monitor;
        }

        [SerializeField]
        private Image _displayPanelPrefab;
        [SerializeField]
        private Image _displayProductPrefab;

        private readonly List<SupplyMonitorEntry> _monitorEntries = new List<SupplyMonitorEntry>();
        private Image _display;
        private WorldClock _worldClock;
        private Text _clockDisplay;

        void Start ()
        {
            InitializeDisplay();
            BindWorldClock();
            OnEveryUpdate += UpdateValues;
            MessageHub.Instance.AddListener(this, ProductSupplyMonitor.CreatedMessageType);
        }
        private void InitializeDisplay()
        {
            _display = Instantiate(_displayPanelPrefab);
            _display.rectTransform.position = new Vector3(10, 700, 0);
            var canvas = GameObject.Find("InfoCanvas");
            _display.transform.SetParent(canvas.transform);
        }

        private void BindWorldClock()
        {
            _worldClock = WorldClock.Instance;
            _clockDisplay = InstantiateNewUIEntry("Date: ");
            _clockDisplay.text = GenerateCurrentTimeString();
        }

        // Adding, and eventually removing monitors happens through messaging
        public void HandleMessage(string type, MessageArgs args)
        {
            var providerArgs = args as ProductSupplyMonitorCreatedMessageArgs;
            if (type != ProductSupplyMonitor.CreatedMessageType || providerArgs == null)
                return;

            OnNextUpdate += f => { AddMonitor(providerArgs.SupplyMonitor); };  
        }

        private void AddMonitor(ProductSupplyMonitor supplyMonitor)
        {
            var valueDisplay = InstantiateNewUIEntry(supplyMonitor.DisplayName);
            valueDisplay.text = supplyMonitor.GetAmountOutput();

            _monitorEntries.Add(new SupplyMonitorEntry { Monitor = supplyMonitor, Text = valueDisplay });
        }


        private Text InstantiateNewUIEntry(string displayName)
        {
            // instantiate UI element
            var contentHolder = _display.transform.FindChild("content_holder");
            var displayProduct = Instantiate(_displayProductPrefab);
            displayProduct.transform.SetParent(contentHolder, false);

            // set its name
            var productName = displayProduct.transform.FindChild("product_name").GetComponent<Text>();
            productName.text = displayName;

            // return the text field to be updated every frame
            return displayProduct.transform.FindChild("product_value").GetComponent<Text>();
        }

        private string GenerateCurrentTimeString()
        {
            return string.Format("{0}.{1}.{2}.{3}.{4}",
                                 _worldClock.CurrentTime.Year,
                                 _worldClock.CurrentTime.Month,
                                 _worldClock.CurrentTime.Week,
                                 _worldClock.CurrentTime.Day,
                                 _worldClock.CurrentTime.Hour);
        }

        private void UpdateValues(float delta)
        {
            _clockDisplay.text = GenerateCurrentTimeString();

            foreach (var entry in _monitorEntries)
            {
                entry.Text.text = entry.Monitor.GetAmountOutput();
            }
        }

        public string Name { get { return "StatsDisplayViewModel"; } }
    }
}
