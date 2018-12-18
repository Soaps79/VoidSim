using System;
using System.Collections.Generic;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
		public string SOName;
		[JsonConverter(typeof(StringEnumConverter))]
		public ShipStatus Status;
		public TickerData Ticker;
		public ShipNavigationData Navigation;
		public CargoManifestBookData ManifestBook;
		public TrafficShipData TrafficShipData;
	    public ProductInventoryData InventoryData;
	}

	public class Ship : ISerializeData<ShipData>
	{
		public ShipSize Size;
		public ShipStatus Status { get; private set; }

		// deal with capacity later
		// ideally, extract simpler version from Inventory and use here
		public int MaxCapacity { get; private set; }
		public int CurrentSpaceUsed { get; private set; }
		public List<ProductAmount> ProductCargo = new List<ProductAmount>();
	    public ProductInventory Inventory = new ProductInventory();

		public CargoManifestBook ManifestBook;
		public ShipNavigation Navigation { get; private set; }
		public TrafficShip TrafficShip { get; private set; }
        public CargoCarrier CargoCarrier { get; private set; } = new CargoCarrier();
		public string Name { get; set; }

		public bool CanTakeCargo(CargoManifest manifest)
		{
			return true;
		}

		public Action OnHoldBegin;
		public Action OnTransitBegin;
		public Ticker Ticker = new Ticker();
		private ShipSO _scriptable;
	    public int ManifestCount;

		public void SetScriptable(ShipSO scriptable)
		{
			_scriptable = scriptable;
        }

		public void Initialize(ShipNavigation navigation)
		{
			Navigation = navigation;
			Navigation.ParentShip = this;

		    if(_scriptable != null)
		        Inventory.SetGlobalMax(_scriptable.MaxCargo);
		    Inventory.DefaultProductCapacity = 1000;
		    Inventory.Initialize(ProductLookup.Instance, false);

            ManifestBook = new CargoManifestBook();
            CargoCarrier.Initialize(Inventory, Navigation, ManifestBook);
		    ManifestBook.OnContentsUpdated += () => { ManifestCount = ManifestBook.ActiveManifests.Count; };
		}

		public void CompleteVisit()
		{
			Navigation.CompleteDestination();
			Status = ShipStatus.Transit;
            // needs to be here for initial use
            OnTransitBegin?.Invoke();
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

            OnHoldBegin?.Invoke();
            return true;
		}

		private void CreateTrafficShip()
		{
			TrafficShip = GameObject.Instantiate(_scriptable.TrafficShipPrefab);
			TrafficShip.SetScriptable(_scriptable);
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
		public void Initialize(ShipNavigation navigation, ShipData data)
		{
			Navigation = navigation;
			Navigation.ParentShip = this;
			Ticker = new Ticker(data.Ticker);
            Inventory.Initialize(data.InventoryData, ProductLookup.Instance, false);
		    ManifestBook = new CargoManifestBook(data.ManifestBook);
            CargoCarrier.Initialize(Inventory, Navigation, ManifestBook);

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
				SOName = _scriptable.name,
				Status = Status,
				Ticker = Ticker.GetData(),
				Navigation = Navigation.GetData(),
				ManifestBook = ManifestBook.GetData(),
				TrafficShipData = Status == ShipStatus.Traffic ? TrafficShip.GetData() : null,
                InventoryData = Inventory.GetData()
			};
		}
		#endregion
	}
}