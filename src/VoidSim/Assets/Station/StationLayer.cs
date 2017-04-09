using Assets.WorldMaterials;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station
{
    public class StationLayer : QScript, IMessageListener
    {
        private Station _parentStation;
        private Inventory _inventory;
        public LayerType LayerType;

        public void Initialize(Station parentStation, Inventory inventory)
        {
            _parentStation = parentStation;
            _inventory = inventory;
        }

        // Use this for initialization
        void Start ()
        {
            MessageHub.Instance.AddListener(this, PlaceableMessages.PlaceablePlacedMessageName);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type != PlaceableMessages.PlaceablePlacedMessageName)
                return;

            var pArgs = args as PlaceablePlacedArgs;
            if (pArgs == null || pArgs.ObjectPlaced.Layer != LayerType)
                return;

            pArgs.ObjectPlaced.transform.SetParent(this.transform);
            HandlePlaceableAdd(pArgs.ObjectPlaced);
        }

        private void HandlePlaceableAdd(Placeable objectPlaced)
        {
            var automated = objectPlaced.GetComponent<AutomatedContainer>();
            if(automated != null)
                automated.Initialize(_inventory);
        }

        public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
    }
}
