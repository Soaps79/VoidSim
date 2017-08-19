using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.UI;
using Messaging;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Station.UI
{
    /// <summary>
    /// Feeds a UI panel that shows information that is constantly relevant to the player.
    /// Listens for ProductSupplyMonitor created messages, and adds their output to the UI display
    /// Logic will need to be written so that the panel itself resizes to accomodate additional values.
    /// 
    /// Possible future usage would be for the player to add and remove products to the display as they come and go in relevency.
    /// </summary>
    public class StatsDisplayViewModel : QScript, IMessageListener
    {
        // acts as a binding between UI elements and inventory
        private class SupplyMonitorEntry
        {
            public TextMeshProUGUI Text;
            public ProductSupplyMonitor Monitor;
        }

        [SerializeField] private Image _displayPanelPrefab;
        [SerializeField] private Image _displayTimePrefab;
        [SerializeField] private Image _displayProductPrefab;
	    [SerializeField] private Image _displayMoodPrefab;

		private readonly List<SupplyMonitorEntry> _monitorEntries = new List<SupplyMonitorEntry>();
        private Image _display;
        private IWorldClock _worldClock;
        private TextMeshProUGUI _clockDisplay;

        void Start ()
        {
            InitializeDisplay();
            BindWorldClock();
            OnEveryUpdate += UpdateValues;
            Locator.MessageHub.AddListener(this, ProductSupplyMonitor.CreatedMessageType);
        }
        private void InitializeDisplay()
        {
            var canvas = GameObject.Find("InfoCanvas");
            _display = Instantiate(_displayPanelPrefab, canvas.transform, false);
        }

        private void BindWorldClock()
        {
            _worldClock = Locator.WorldClock;

            // instantiate UI element
            var contentHolder = _display.transform.Find("content_holder");
            var displayProduct = Instantiate(_displayTimePrefab);
            displayProduct.transform.SetParent(contentHolder, false);

            // return the text field to be updated every frame
            _clockDisplay = displayProduct.transform.Find("product_value").GetComponent<TextMeshProUGUI>();
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
            var valueDisplay = InstantiateNewUIEntry(supplyMonitor);
            valueDisplay.text = supplyMonitor.GetAmountOutput();

            _monitorEntries.Add(new SupplyMonitorEntry { Monitor = supplyMonitor, Text = valueDisplay });
	        if (_monitorEntries.Count == 2)
		        AddMoodMonitor();
        }

	    private void AddMoodMonitor()
	    {
            var contentHolder = _display.transform.Find("content_holder");
		    var mood = Instantiate(_displayMoodPrefab);
		    var viewmodel = mood.GetComponent<PopMoodViewModel>();
            var popControl = GameObject.Find("population_control").GetComponent<PopulationControl>();
		    viewmodel.Bind(popControl);
			mood.transform.SetParent(contentHolder, false);
	    }


	    private TextMeshProUGUI InstantiateNewUIEntry(ProductSupplyMonitor monitor)
        {
            // instantiate UI element
            var contentHolder = _display.transform.Find("content_holder");
            var displayProduct = Instantiate(_displayProductPrefab);
            displayProduct.transform.SetParent(contentHolder, false);

            // set its name
            var product_icon = displayProduct.transform.Find("product_image").GetComponent<Image>();
            product_icon.sprite = monitor.Product.Icon;

            // return the text field to be updated every frame
            return displayProduct.transform.Find("product_value").GetComponent<TextMeshProUGUI>();
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
