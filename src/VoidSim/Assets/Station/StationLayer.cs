using Assets.Placeables;
using Assets.WorldMaterials;
using Messaging;
using QGame;

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
            MessageHub.Instance.AddListener(this, PlaceableMessages.PlaceablePlaced);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == PlaceableMessages.PlaceablePlaced)
                HandlePlaceableAdd(args as PlaceablePlacedArgs);
        }

        private void HandlePlaceableAdd(PlaceablePlacedArgs placed)
        {
            if (placed == null || placed.Layer != LayerType)
                return;

            placed.ObjectPlaced.transform.SetParent(transform);
        }

        public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
    }
}
