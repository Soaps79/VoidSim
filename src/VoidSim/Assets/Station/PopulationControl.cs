using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    /// <summary>
    /// Placeholder for where pop will be managed
    /// </summary>
    public class PopulationControl : QScript, IMessageListener
    {
        [SerializeField] private int _totalCapacity;
        [SerializeField] private int _initialCapacity;
        private readonly List<PopHousing> _housing = new List<PopHousing>();
        private const string POPULATION_PRODUCT_NAME = "Population";
        private Inventory _inventory;
        private int _populationProductId;

        public void Initialize(Inventory inventory, int initialCapacity = 0)
        {
            _initialCapacity = initialCapacity;
            _inventory = inventory;
            var pop = ProductLookup.Instance.GetProduct(POPULATION_PRODUCT_NAME);
            _populationProductId = pop.ID;

            if (_initialCapacity > 0)
                _inventory.SetProductMaxAmount(_populationProductId, _initialCapacity);

			MessageHub.Instance.AddListener(this, PopHousing.MessageName);

			// remove when housing serialization is in place
			Locator.LastId.Reset("pop_housing");
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
    }
}