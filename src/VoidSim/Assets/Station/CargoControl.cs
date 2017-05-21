using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Station.UI;
using Assets.WorldMaterials;
using Messaging;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Station
{
    public class CargoControl : QScript, IMessageListener
    {
        private Inventory _inventory;
        private InventoryReserve _reserve;
        private string _clientName;

        private readonly List<CargoBay> _cargoBays = new List<CargoBay>();

	    public Action<CargoBay> OnBayAdded;
	    [SerializeField] private TransactionText _textPrefab;

        void Start()
        {
            MessageHub.Instance.AddListener(this, LogisticsMessages.ShipBerthsUpdated);
        }

        public void Initialize(Inventory inventory, InventoryReserve reserve, string clientName)
        {
            _inventory = inventory;
            _reserve = reserve;
            _clientName = clientName;
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == LogisticsMessages.ShipBerthsUpdated && args != null)
                HandleShipBerthUpdate(args as ShipBerthsMessageArgs);
        }

        private void HandleShipBerthUpdate(ShipBerthsMessageArgs args)
        {
            if (args == null)
                throw new UnityException("CargoControl got bad berths message args");

            if (args.WereRemoved)
                _cargoBays.RemoveAll(i => args.Berths.Any(j => j.name == i.Name));
            else
                args.Berths.ForEach(CreateCargoBay);
        }

        private void CreateCargoBay(ShipBerth berth)
        {
            var go = new GameObject();
            var cargoBay = go.AddComponent<CargoBay>();
			cargoBay.transform.SetParent(transform, true);
            cargoBay.Initialize(berth, _inventory, _reserve, _clientName, _textPrefab);
            _cargoBays.Add(cargoBay);

	        if (OnBayAdded != null)
		        OnBayAdded(cargoBay);
        }

        public string Name { get { return "CargoControl"; } }
    }
}