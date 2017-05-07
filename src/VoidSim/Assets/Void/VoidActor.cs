using Assets.Placeables;
using Assets.Placeables.Nodes;
using Assets.Station;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Void
{
    public class VoidActor : QScript, ITransitLocation
    {
        [Inject] private WorldClock _worldClock;
        [Inject] private ProductLookup _productLookup;

        [SerializeField] private TraderRequestsSO _tradeRequests;
        [SerializeField] private string _clientName;
        private ProductTradeAutomater _automater;
        private ProductTrader _trader;

        void Start()
        {
            InstantiateTransitRegister();
            InstantiateVoidTrader();
            MessageHub.Instance.QueueMessage(TransitMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
        }

        private void InstantiateTransitRegister()
        {
            var go = new GameObject();
            go.transform.SetParent(transform);
            go.name = "transit_register";
            var register = go.AddComponent<TransitRegister>();
        }

        private void InstantiateVoidTrader()
        {
            var go = new GameObject();
            go.transform.SetParent(transform);
            go.name = "void_trader";
            _trader = go.AddComponent<ProductTrader>();
            _trader.OnProvideMatch += HandleProvideMatch;
            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

            _automater = go.AddComponent<ProductTradeAutomater>();
            _automater.Initialize(_tradeRequests, _trader, _worldClock);
        }

        private void HandleProvideMatch(TradeInfo info)
        {
            // create ship and add to transit
            MessageHub.Instance.QueueMessage(TransitMessages.TransitRequested, new TransitRequestedMessageArgs
            {
                TravelingFrom = this,
                TravelingTo = info.Consumer.ClientName,
                Ship = new Ship { Type = ShipType.Corvette }
            });
        }

        public string ClientName { get { return _clientName; } }
        public void OnTransitComplete(TransitRegister.Entry entry)
        {
            
        }
    }
}