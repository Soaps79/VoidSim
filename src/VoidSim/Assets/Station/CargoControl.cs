using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station.UI;
using Assets.WorldMaterials;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class CargoControl : QScript, IMessageListener
	{
		private WorldMaterials.StationInventory _stationInventory;
		private InventoryReserve _reserve;

		private readonly List<CargoBay> _cargoBays = new List<CargoBay>();

		public Action<CargoBay> OnBayAdded;
		[SerializeField] private TransactionText _textPrefab;
		private PopulationControl _popControl;

		void Start()
		{
			Locator.MessageHub.AddListener(this, LogisticsMessages.ShipBerthsUpdated);
		}

		public void Initialize(WorldMaterials.StationInventory stationInventory, InventoryReserve reserve, PopulationControl popControl)
		{
			_stationInventory = stationInventory;
			_reserve = reserve;
			_popControl = popControl;
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

			args.Berths.ForEach(CreateCargoBay);
			args.ShipBay.OnRemove += HandleShipBayRemove;
		}

		private void HandleShipBayRemove(ShipBay shipBay)
		{
			var names = shipBay.Berths.Select(i => i.Name);
			var bays = _cargoBays.Where(i => names.Contains(i.Name)).ToList();
			if (!bays.Any()) return;
			foreach (var cargoBay in bays)
			{
				_cargoBays.Remove(cargoBay);
				Destroy(cargoBay);
			}
		}

		private void CreateCargoBay(ShipBerth berth)
		{
			var go = new GameObject();
			var cargoBay = go.AddComponent<CargoBay>();
			cargoBay.transform.SetParent(transform, true);
			cargoBay.Initialize(berth, _stationInventory, _reserve, _textPrefab);
			_cargoBays.Add(cargoBay);
		    if (_popControl != null)
		        cargoBay.OnCargoManifestComplete += _popControl.HandleManifestComplete;

			if (OnBayAdded != null)
				OnBayAdded(cargoBay);
		}

		public string Name { get { return "CargoControl"; } }
	}
}