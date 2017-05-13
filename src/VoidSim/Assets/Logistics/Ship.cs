using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics
{
    public class ShipNavigation
    {
        public ITransitLocation LastDeparted { get; private set; }
        public ITransitLocation CurrentDestination { get; private set; }

        public Ship ParentShip;

        private readonly Queue<ITransitLocation> _locations = new Queue<ITransitLocation>();

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

        private void CycleLocations()
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

    public class TradeManifest
    {
        public int Id;
        public string Buyer;
        public string Seller;
        public ProductAmount ProductAmount;
        public int Currency;
    }

    public class TradeManifestBook
    {
        public List<TradeManifest> ActiveManifests { get; private set; }

        public List<TradeManifest> GetBuyerManifests(string clientName)
        {
            return ActiveManifests.Where(i => i.Buyer == clientName).ToList();
        }

        public List<TradeManifest> GetSellerManifests(string clientName)
        {
            return ActiveManifests.Where(i => i.Seller == clientName).ToList();
        }

        public TradeManifestBook()
        {
            ActiveManifests = new List<TradeManifest>();
        }

        public void Add(TradeManifest manifest)
        {
            if (manifest != null)
                ActiveManifests.Add(manifest);
        }

        public void Close(int id)
        {
            ActiveManifests.RemoveAll(i => i.Id == id);
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

        public TradeManifestBook ManifestBook = new TradeManifestBook();
        private ShipNavigation _navigation;
        private ShipBerth _berth;
        public GameObject TrafficShipPrefab;
        public TrafficShip TrafficShip { get; private set; }

        public void Initialize(ShipNavigation navigation, GameObject prefab)
        {
            _navigation = navigation;
            TrafficShipPrefab = prefab;
            if(TrafficShipPrefab == null)
                throw new UnityException("Ship got bad trafficship prefab");
        }

        public void AddManifest(TradeManifest manifest)
        {
            if(manifest != null)
                ManifestBook.Add(manifest);

            Debug.Log(string.Format("Ship given manifest: {0} to {1}, {2} x{3}", 
                manifest.Seller, manifest.Buyer, manifest.ProductAmount.ProductId, manifest.ProductAmount.Amount));
        }

        public void CompleteVisit()
        {
            _navigation.CompleteDestination();
        }

        public bool AcknowledgeBerth(ShipBerth shipBerth)
        {
            CreateTrafficShip(shipBerth);
            return true;
        }

        private void CreateTrafficShip(ShipBerth berth)
        {
            // will be replaced with a prefab
            var go = Object.Instantiate(TrafficShipPrefab);
            TrafficShip = go.GetComponent<TrafficShip>();
            TrafficShip.Initialize(this, berth);
            TrafficShip.BeginApproach();
        }

        public void OnTrafficComplete()
        {
            // adjust manifests
            GameObject.Destroy(TrafficShip);
            TrafficShip = null;
            CompleteVisit();
        }
    }
}