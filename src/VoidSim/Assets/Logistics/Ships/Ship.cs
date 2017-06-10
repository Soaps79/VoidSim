using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using UnityEngine;

namespace Assets.Logistics.Ships
{
	public enum ShipStatus
	{
		New, Transit, Traffic, Hold
	}

	public class ShipData
	{
		public string Name;
		public ShipStatus Status;
		public TickerData Ticker;
		public ShipNavigationData Navigation;
		public CargoManifestBookData ManifestBook;
		public TrafficShipData TrafficShipData;
	}

	public class Ship : ISerializeData<ShipData>
	{
		public ShipSize Size;
		public string CurrentDestination;
		public ShipStatus Status { get; private set; }

		// deal with capacity later
		// ideally, extract simpler version from Inventory and use here
		public int MaxCapacity { get; private set; }
		public int CurrentSpaceUsed { get; private set; }
		public List<ProductAmount> ProductCargo = new List<ProductAmount>();

		public CargoManifestBook ManifestBook = new CargoManifestBook();
		public ShipNavigation Navigation { get; private set; }
		public GameObject TrafficShipPrefab;
		public TrafficShip TrafficShip { get; private set; }
		public string Name { get; set; }

		public Action OnHoldBegin;
		public Action OnTransitBegin;
		public Ticker Ticker = new Ticker();

		public void Initialize(ShipNavigation navigation, GameObject prefab, ShipStatus status = ShipStatus.New)
		{
			Navigation = navigation;
			Navigation.ParentShip = this;
			TrafficShipPrefab = prefab;
			if(TrafficShipPrefab == null)
				throw new UnityException("Ship got bad trafficship prefab");
		}

		public void AddManifest(CargoManifest manifest)
		{
			if (manifest == null)
				return;

			ManifestBook.Add(manifest);

			var s = string.Format("{0} given manifest {1}:\t {2} to {3}\t {4} x{5}", Name, manifest.Id,
				manifest.Seller, manifest.Buyer, manifest.ProductAmount.ProductId, manifest.ProductAmount.Amount);
			UberDebug.LogChannel(LogChannels.Trade, s);
		}

		public void CompleteVisit()
		{
			Navigation.CompleteDestination();
			Status = ShipStatus.Transit;
			// needs to be here for initial use
			if (OnTransitBegin != null)
				OnTransitBegin();
		}

		// This could probably be named better... as of now, Traffic is like a specialized Hold
		public bool BeginHold(ShipBerth shipBerth, List<Vector3> waypoints)
		{
			if (shipBerth != null)
			{
				CreateTrafficShip();
				TrafficShip.Initialize(this, shipBerth, waypoints);
				TrafficShip.BeginApproach();
				Status = ShipStatus.Traffic;
			}
			else
			{
				Status = ShipStatus.Hold;
			}

			if (OnHoldBegin != null)
				OnHoldBegin();
			return true;
		}

		private void CreateTrafficShip()
		{
			var go = GameObject.Instantiate(TrafficShipPrefab);
			go.TrimCloneFromName();
			TrafficShip = go.GetComponent<TrafficShip>();
		}

		public void CompleteTraffic()
		{
			// adjust manifests
			if (TrafficShip != null)
			{
				GameObject.Destroy(TrafficShip.gameObject);
				TrafficShip = null;
			}
			
			CompleteVisit();
		}

		#region Serialization
		public void Initialize(ShipNavigation navigation, GameObject prefab, ShipData data)
		{
			Navigation = navigation;
			Navigation.ParentShip = this;
			TrafficShipPrefab = prefab;
			Ticker = new Ticker(data.Ticker);
			if (TrafficShipPrefab == null)
				throw new UnityException("Ship got bad trafficship prefab");
			ManifestBook = new CargoManifestBook(data.ManifestBook);

			// possibly a better place to put this
			foreach (var manifest in ManifestBook.ActiveManifests)
			{
				manifest.TradeManifest = TradeMonitor.Instance.GetTradeManifest(manifest.TradeManifestId);
			}

			Status = data.Status;
			if (Status == ShipStatus.Traffic)
			{
				CreateTrafficShip();
				TrafficShip.Initialize(this, data.TrafficShipData);
			}
			else if (Status == ShipStatus.Transit)
			{
				Navigation.BeginTrip(true);
			}
		}


		public ShipData GetData()
		{
			return new ShipData
			{
				Name = Name,
				Status = Status,
				Ticker = Ticker.GetData(),
				Navigation = Navigation.GetData(),
				ManifestBook = ManifestBook.GetData(),
				TrafficShipData = Status == ShipStatus.Traffic ? TrafficShip.GetData() : null
				// need to serialize cargo manifests
				// need to serialize ticker
			};
		}
		#endregion
	}
}