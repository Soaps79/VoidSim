using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Placeables.HardPoints;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine.UI;

namespace Assets.Placeables.Placement
{
    [Serializable]
    public class PrePlacedPlaceable
    {
        public PlaceableScriptable Placeable;
        public int HardPointNumber;
    }

    public class PrePlacer : QScript, IMessageListener
    {
        public bool IsOn;
        public List<PrePlacedPlaceable> Placeables;
        private List<PrePlacedPlaceable> _toPlace;

        void Start()
        {
            if (!IsOn || Placeables == null || !Placeables.Any() || Locator.Serialization.IsLoading)
            {
                enabled = false;
                return;
            }

            _toPlace = Placeables.ToList();
            Locator.MessageHub.AddListener(this, HardPointGroup.MessageName);
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == HardPointGroup.MessageName && args != null)
                HandleHardPointAdd(args as HardPointGroupUpdateMessage);
        }

        private void HandleHardPointAdd(HardPointGroupUpdateMessage args)
        {
            if (args == null || _toPlace.All(i => i.Placeable.Layer != args.Layer))
                return;

            var currentLayer = _toPlace.Where(i => i.Placeable.Layer == args.Layer);
            foreach (var hardPoint in args.Group.GetAvailableHardPoints())
            {
                var placeable = currentLayer.FirstOrDefault(i => i.HardPointNumber == hardPoint.Number);
                if (placeable != null)
                {
                    Placer.PlaceObject(placeable.Placeable, hardPoint.transform.position, hardPoint.name);
                    _toPlace.Remove(placeable);
                }
            }

            if (!_toPlace.Any())
                enabled = false;
        }

        public string Name { get { return "PrePlacer"; } }
    }
}