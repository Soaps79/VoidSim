using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Ships;
using Assets.Logistics.Transit;
using Assets.Scripts;
using QGame;
using UnityEngine;

namespace Logistics.Transit
{
    // LocationDrivers are objects who are given the first shot at handling ships arriving at the Location
    // Otherwise, ships will be placed on a simple hold
    public interface ILocationDriver
    {
        bool TryHandleArrival(Ship ship);
    }

    [RequireComponent(typeof(ShipHolder))]
    public class TransitLocation : QScript
    {
        [SerializeField] private bool _isSimpleHold;
        [SerializeField] private string _clientName;
        private ShipHolder _holder;
        private readonly List<ILocationDriver> _drivers = new List<ILocationDriver>();

        public bool IsSimpleHold { get { return _isSimpleHold;  } }
        public string ClientName { get { return _clientName;  } }

        public List<Ship> Ships = new List<Ship>();

        private void Awake()
        {
            _holder = GetComponent<ShipHolder>();
        }

        private void Start()
        {
            Locator.MessageHub.QueueMessage(
                LogisticsMessages.RegisterLocation, new TransitLocationMessageArgs{ TransitLocation = this });
        }

        public Action<Ship> OnTransitArrival;
        // offer ship to drivers, otherwise put it on hold
        public void HandleTransitArrival(Ship ship)
        {
            Ships.Add(ship);
            if (!_drivers.Any(i => i.TryHandleArrival(ship)))
                _holder.BeginHold(ship);
            if (OnTransitArrival != null) OnTransitArrival(ship);
        }

        public Action<Ship> OnTransitDeparture;
        public void HandleTransitDeparture(Ship ship)
        {
            Ships.Remove(ship);
            if (OnTransitDeparture != null) OnTransitDeparture(ship);
        }

        public void HandleCargoRequested(CargoManifest manifest)
        {
            
        }

        public Action<Ship> OnResume;
        // if it is on hold, resume hold
        // if not, driver should pick it up from callback
        public void Resume(Ship ship)
        {
            if (ship.Status == ShipStatus.Hold)
            {
                _holder.BeginHold(ship, true);
                return;
            }

            Ships.Add(ship);
            if (OnResume != null) OnResume(ship);
        }

        public bool HasShipsOnHold()
        {
            return _holder.Count > 0;
        }

        public List<Ship> GetShipsFromHold(int count)
        {
            return _holder.RemoveShips(count);
        }

        public List<Ship> GetShipList()
        {
            return Ships;
        }

        public void RegisterDriver(ILocationDriver locationDriver)
        {
            _drivers.Add(locationDriver);
        }
    }
}