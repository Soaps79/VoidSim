﻿using Assets.Placeables.Nodes;
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
            if (type == PlaceableMessages.PlaceablePlacedMessageName)
                HandlePlaceableAdd(args as PlaceablePlacedArgs);
        }

        private void HandlePlaceableAdd(PlaceablePlacedArgs placed)
        {
            if (placed == null)
                return;

            // this was used for energy, figure its bound to come in handy
        }

        public string Name { get { return string.Format("StationLayer {0}", LayerType); } }
    }
}
