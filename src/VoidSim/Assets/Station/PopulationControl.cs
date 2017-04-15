using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
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
        private readonly List<PopHousing> _housing = new List<PopHousing>();
        private const string POPULATION_PRODUCT_NAME = "Population";
        private Inventory _inventory;
        private int _populationProductId;

        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
            var pop = ProductLookup.Instance.GetProduct(POPULATION_PRODUCT_NAME);
            _populationProductId = pop.ID;
            MessageHub.Instance.AddListener(this, PopHousing.MessageName);
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

            _housing.Add(args.PopHousing);
            UpdateCapacity();
            _inventory.SetProductMaxAmount(_populationProductId, _totalCapacity);
        }

        private void UpdateCapacity()
        {
            _totalCapacity = _housing.Sum(i => i.Capacity);

        }

        public string Name
        {
            get { return "PopulationControl"; }
        }
    }
}