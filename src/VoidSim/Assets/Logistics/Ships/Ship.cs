using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using Messaging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Logistics.Ships
{
	public class ShipNavigation
	{
		public ITransitLocation LastDeparted { get; private set; }
		public ITransitLocation CurrentDestination { get; private set; }

		public Ship ParentShip;

		private readonly Queue<ITransitLocation> _locations = new Queue<ITransitLocation>();

		// initializes with empty location objects
		public ShipNavigation()
		{
			LastDeparted = new EmptyTransitLocation();
			CurrentDestination = new EmptyTransitLocation();
		}

		public void BeginTrip()
		{
			MessageHub.Instance.QueueMessage(LogisticsMessages.TransitRequested, new TransitRequestedMessageArgs
			{
				Ship = ParentShip,
				TravelingFrom = LastDeparted.ClientName,
				TravelingTo = CurrentDestination.ClientName
			});
		}

		public void CompleteDestination()
		{
			// will currently cycle through all known destination
			CycleLocations();
			BeginTrip();
		}

		public void CycleLocations()
		{
			LastDeparted = _locations.Dequeue();
			_locations.Enqueue(LastDeparted);
			CurrentDestination = _locations.Peek();
		}

		public void AddLocation(ITransitLocation location)
		{
			if(location == null)
				throw new UnityException("Ship navigation given bad location data");

			_locations.Enqueue(location);
		}
	}

	
	public class Ticker
	{
		public float TotalTicks;
		public float ElapsedTicks;

		public float TimeRemainingAsZeroToOne
		{
			get { return TotalTicks > 0 ? ElapsedTicks / TotalTicks : 1; }
		}

		public bool IsComplete { get { return ElapsedTicks >= TotalTicks; } }

		public void Reset(float newTotal = 0)
		{
			ElapsedTicks = 0;
			if (newTotal != 0)
				TotalTicks = newTotal;
		}
	}

	public class Ship
	{
		public ShipSize Size;
		public string CurrentDestination;

		// deal with capacity later
		public int MaxCapacity { get; private set; }
		public int CurrentSpaceUsed { get; private set; }
		public List<ProductAmount> ProductCargo = new List<ProductAmount>();

		public CargoManifestBook ManifestBook = new CargoManifestBook();
		public ShipNavigation Navigation { get; private set; }
		private ShipBerth _berth;
		public GameObject TrafficShipPrefab;
		public TrafficShip TrafficShip { get; private set; }
		public string Name { get; set; }

		public Action OnHoldBegin;
		public Action OnTransitBegin;
		public Ticker Ticker = new Ticker();

		public void Initialize(ShipNavigation navigation, GameObject prefab)
		{
			Navigation = navigation;
			TrafficShipPrefab = prefab;
			if(TrafficShipPrefab == null)
				throw new UnityException("Ship got bad trafficship prefab");
		}

		public void AddManifest(CargoManifest manifest)
		{
			if(manifest != null)
				ManifestBook.Add(manifest);

			Debug.Log(string.Format("Ship given manifest: {0} to {1}, {2} x{3}", 
				manifest.Seller, manifest.Buyer, manifest.ProductAmount.ProductId, manifest.ProductAmount.Amount));
		}

		public void CompleteVisit()
		{
			Navigation.CompleteDestination();
			// needs to be here for initial use
			if (OnTransitBegin != null)
				OnTransitBegin();
		}

		public bool BeginHold(ShipBerth shipBerth, List<Vector3> waypoints)
		{
			if(shipBerth != null)
				CreateTrafficShip(shipBerth, waypoints);

			if (OnHoldBegin != null)
				OnHoldBegin();
			return true;
		}

		private void CreateTrafficShip(ShipBerth berth, List<Vector3> waypoints)
		{
			var go = Object.Instantiate(TrafficShipPrefab);
			go.TrimCloneFromName();
			TrafficShip = go.GetComponent<TrafficShip>();
			TrafficShip.Initialize(this, berth, waypoints);
			TrafficShip.BeginApproach();
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
	}
}