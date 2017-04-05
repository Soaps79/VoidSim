using System;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station.UI
{
    /// <summary>
    /// Currently, this is all just to get some values on the screen. It should be made more dynamic, 
    /// so a list of IProvideStats concrete classes can display any data they want in this panel.
    /// Logic will need to be written so that the panel itself resizes to accomodate additional values.
    /// Further, the providers should provide their icon instead of string Name.
    /// </summary>

    public static class StatsMessages
    {
        public const string StatProviderCreated = "StatProviderCreated";
    }

    public class StatProviderCreatedMessageArgs : MessageArgs
    {
        public StatProvider ValueProvider;
    }

    public class StatProvider
    {
        public string Name;
        public Func<string> Value;
    }

    public class StatsDisplayViewModel : QScript, IMessageListener
    {
        [SerializeField]
        private Image _displayPanelPrefab;
        [SerializeField]
        private Image _displayProductPrefab;


        private Image _display;

        private Text _energyValueDisplay;

        private StatProvider _energyGrid;

        void Start ()
        {
            // this is all gross, make more dynamic
            _display = Instantiate(_displayPanelPrefab);
            _display.rectTransform.position = new Vector3(10, 550, 0);
            var canvas = GameObject.Find("InfoCanvas");
            _display.transform.SetParent(canvas.transform);
            var contentHolder = _display.transform.FindChild("content_holder");

            var displayProduct = Instantiate(_displayProductPrefab);
            
            var productName = displayProduct.transform.FindChild("product_name").GetComponent<Text>();
            productName.text = "Energy: ";

            displayProduct.transform.SetParent(contentHolder, false);

            _energyValueDisplay = displayProduct.transform.FindChild("product_value").GetComponent<Text>();

            MessageHub.Instance.AddListener(this, StatsMessages.StatProviderCreated);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            var providerArgs = args as StatProviderCreatedMessageArgs;
            if (type != StatsMessages.StatProviderCreated || providerArgs == null)
                return;

            _energyGrid = providerArgs.ValueProvider;
            OnEveryUpdate += UpdateEnergyValue;
        }

        private void UpdateEnergyValue(float obj)
        {
            _energyValueDisplay.text = _energyGrid.Value();
        }

        public string Name { get { return "StatsDisplayViewModel"; } }
    }
}
