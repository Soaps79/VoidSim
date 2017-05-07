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
            MessageHub.Instance.QueueMessage(TransitMessages.TransitRequested, new TransitRequestedMessageArgs
            {
                Ship = ParentShip,
                TravelingFrom = LastDeparted.ClientName,
                TravelingTo = CurrentDestination.ClientName
            });
        }

        public void CompleteDestination()
        {
            // will currently cycle through all known destination
            LastDeparted = _locations.Dequeue();
            _locations.Enqueue(LastDeparted);
            CurrentDestination = _locations.Peek();
            BeginTrip();
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
        public List<ProductAmount> Products;
        public int Currency;
    }

    public class Ship
    {
        public ShipType Type;
        public string CurrentDestination;

        // deal with capacity later
        public int MaxCapacity { get; private set; }
        public int CurrentSpaceUsed { get; private set; }
        public List<ProductAmount> ProductCargo = new List<ProductAmount>();

        private readonly List<TradeManifest> _activeManifests = new List<TradeManifest>();
        private ShipNavigation _navigation;

        public void Initialize(ShipNavigation navigation)
        {
            _navigation = navigation;
        }

        public void AddManifest(TradeManifest manifest)
        {
            if(manifest != null)
                _activeManifests.Add(manifest);

            Debug.Log(string.Format("Ship given manifest: {0} to {1}, {2} x{3}", 
                manifest.Seller, manifest.Buyer, manifest.Products.First().ProductId, manifest.Products.First().Amount));
        }

        public List<TradeManifest> GetBuyerManifests(string clientName)
        {
            return _activeManifests.Where(i => i.Buyer == clientName).ToList();
        }

        public List<TradeManifest> GetSellerManifests(string clientName)
        {
            return _activeManifests.Where(i => i.Seller == clientName).ToList();
        }

        public void CloseManifest(int id)
        {
            _activeManifests.RemoveAll(i => i.Id == id);
        }

        public void CompleteVisit()
        {
            _navigation.CompleteDestination();
        }
    }
}