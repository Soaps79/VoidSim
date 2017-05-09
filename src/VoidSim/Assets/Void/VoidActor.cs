using System.Collections.Generic;
using Assets.Logistics;
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

        [SerializeField] private ProductValueLookup _valueLookup;
        [SerializeField] private TraderRequestsSO _tradeRequests;
        private const string _clientName = "Void";
        private ProductTradeAutomater _automater;
        private ProductTrader _trader;

        void Start()
        {
            _valueLookup = ProductValueLookup.Instance;
            InstantiateVoidTrader();
            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });
            MessageHub.Instance.QueueMessage(LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
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
            _trader.ClientName = ClientName;
            _trader.OnProvideMatch += HandleProvideMatch;
            MessageHub.Instance.QueueMessage(ProductTrader.MessageName, new TraderInstanceMessageArgs { Trader = _trader });

            _automater = go.AddComponent<ProductTradeAutomater>();
            _automater.Initialize(_tradeRequests, _trader, _worldClock);
        }

        private void HandleProvideMatch(TradeInfo info)
        {
            // request cargo for trade
            MessageHub.Instance.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
            {
                Manifest = new TradeManifest
                {
                    Seller = ClientName,
                    Buyer = info.Consumer.ClientName,
                    Currency = _valueLookup.GetValueOfProductAmount(info.ProductId, info.Amount),
                    Products = new List<ProductAmount> { new ProductAmount { ProductId = info.ProductId, Amount = info.Amount } }
                }
            });
        }

        public string ClientName { get { return _clientName; } }
        public void OnTransitArrival(TransitRegister.Entry entry)
        {
            entry.Ship.CompleteVisit();
        }

        public void OnTransitDeparture(TransitRegister.Entry entry)
        {
            
        }
    }
}