using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Logistics.Transit
{
    public class CargoDispatch : QScript, IMessageListener
	{
		private readonly List<CargoCarrier> _carriers = new List<CargoCarrier>();
		private readonly List<CargoManifest> _manifestBacklog = new List<CargoManifest>();

		private void Start()
		{
			Locator.MessageHub.AddListener(this, LogisticsMessages.ShipCreated);
		    Locator.MessageHub.AddListener(this, LogisticsMessages.CargoTransitRequested);
		    var node = StopWatch.AddNode("cargo check", 5.0f);
		    node.OnTick += CheckDistribution;
		}

        private readonly List<CargoCarrier> _attemptingDistribution = new List<CargoCarrier>(10);
	    private void CheckDistribution()
	    {
	        if (!_manifestBacklog.Any())
	            return;

            foreach (var cargoManifest in _manifestBacklog)
	        {
                // try to distribute to all carriers heading to provider
	            _attemptingDistribution.Clear();
                _attemptingDistribution.AddRange(_carriers.Where(i => cargoManifest.Receiver.Equals(i.Navigation.CurrentDestination)));
	            if (_attemptingDistribution.Any())
	                TryDistributeProduct(cargoManifest);
	        }

	        _manifestBacklog.RemoveAll(i => i.RemainingAmount <= 0);
	    }

	    private void TryDistributeProduct(CargoManifest manifest)
        {
            var first = _attemptingDistribution.FirstOrDefault(
	            i => i.IsEmpty() && i.CanPickupProductThisStop(manifest.ProductId) > manifest.RemainingAmount);

	        if (first != null)
	        {
	            first.ManifestBook.Add(new CargoManifest(manifest));
	            manifest.RemainingAmount = 0;
                return;
	        }

	        for (var i = 0; i < _attemptingDistribution.Count; i++)
	        {
	            manifest.RemainingAmount = _attemptingDistribution[i].CanPickupProductThisStop(manifest.ProductId);
	            if (manifest.RemainingAmount <= 0) break;
	        }
	    }

	    public static Ship FindCarrier(List<Ship> ships, CargoManifest manifest)
		{
			var ship = FindShipHeadingTo(ships, manifest);
			//if (ship == null)
			//	ship = FindRandomShipWithRoom(ships);

			return ship;
		}

		private static Ship FindRandomShipWithRoom(List<Ship> ships)
		{
			var rand = Random.Range(0, ships.Count - 1);
			return ships[rand];
		}

		private static Ship FindShipHeadingTo(List<Ship> ships, CargoManifest manifest)
		{
			var valid = ships.Where(i => i.Navigation.CurrentDestination == manifest.Shipper).ToList();
			return valid.Any() ? valid[Random.Range(0, valid.Count - 1)] : null;
		}

		public void HandleMessage(string type, MessageArgs args)
		{
		    if (type == LogisticsMessages.ShipCreated && args != null)
				HandleShipCreated(args as ShipCreatedMessageArgs);

		    if (type == LogisticsMessages.CargoTransitRequested && args != null)
		        HandleCargoRequested(args as CargoTransitRequestedMessageArgs);
        }

		private void HandleShipCreated(ShipCreatedMessageArgs args)
		{
			if(args == null)
				throw new UnityException("CargoDispatch given bad ship created args");

			_carriers.Add(args.Ship.CargoCarrier);
		}

	    public void HandleCargoRequested(CargoTransitRequestedMessageArgs args)
	    {
	        if (args == null || args.Manifest == null)
	            throw new UnityException("CargoDispatch given bad cargo request");

	        _manifestBacklog.Add(args.Manifest);
	    }


        public string Name => "CargoDispatch";
	}
}