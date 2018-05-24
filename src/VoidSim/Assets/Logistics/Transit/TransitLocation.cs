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
    public interface ILocationDriver
    {
        bool TryHandleArrival(Ship ship);
    }

    [RequireComponent(typeof(ShipHolder))]
    // Serves as a node in the transit system
    // If no driver registered or driver does not want to receive ship, will place on hold
    public class TransitLocation : QScript
    {
        [SerializeField] private bool _isSimpleHold;
        [SerializeField] private string _clientName;
        private ShipHolder _holder;
        private readonly List<ILocationDriver> _drivers = new List<ILocationDriver>();

        public string ClientName { get { return _clientName;  } }

        // does this need to be public? It used to...
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


        // This class's interface is wonky
        // Make a decision between triggering callbacks and calling the driver and stick with it 
        public Action<Ship> OnTransitArrival;
        // offer ship to drivers, otherwise put it on hold
        public void HandleTransitArrival(Ship ship)
        {
            Ships.Add(ship);
            if (!_drivers.Any(i => i.TryHandleArrival(ship)))
                _holder.BeginHold(ship);
            OnTransitArrival?.Invoke(ship);
        }

        public Action<Ship> OnTransitDeparture;
        // Not sure if anyone will ever care about Departure directly, but feels wrong to not expose this
        public void HandleTransitDeparture(Ship ship)
        {
            Ships.Remove(ship);
            OnTransitDeparture?.Invoke(ship);
        }

        public void HandleCargoRequested(CargoManifest manifest)
        {
            // Implement me during Distribution v1
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

        // TODO: Give another pass to the driver > transitlocation > holder relationship
        public bool HasShipsOnHold()
        {
            return _holder.Count > 0;
        }

        public List<Ship> GetShipsFromHold(int count)
        {
            return _holder.RemoveShips(count);
        }

        public void RegisterDriver(ILocationDriver locationDriver)
        {
            _drivers.Add(locationDriver);
        }
    }
}