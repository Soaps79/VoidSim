using System.Collections.Generic;
using System.Linq;
using Assets.Logistics.Ships;
using Logistics.Transit;
using QGame;

namespace Assets.Logistics.Transit
{
    /// <summary>
    /// Represents a single TransitLocation in the cargo distribution system
    /// Holds a list of CargoManifests and distributes them to ships at the location
    /// </summary>
    public class CargoDispatch : QScript
	{
		private readonly List<CargoManifest> _manifestBacklog = new List<CargoManifest>();
	    private TransitLocation _location;

        private void Start()
		{
		    _location = GetComponent<TransitLocation>();
            var node = StopWatch.AddNode("cargo check", 5.0f);
		    node.OnTick += CheckDistribution;
		}

        private void CheckDistribution()
	    {
	        if (!_manifestBacklog.Any())
	            return;


            foreach (var cargoManifest in _manifestBacklog)
            {
                var hasRoom =
                    _location.Ships.FirstOrDefault(
                        i => i.CargoCarrier.CanPickupProductThisStop(cargoManifest.ProductId) >
                             cargoManifest.RemainingAmount);

                if (hasRoom != null)
                {
                    // copying for now, make this better
                    hasRoom.CargoCarrier.AddManifestAndProduct(new CargoManifest(cargoManifest));
                    cargoManifest.RemainingAmount = 0;
                }
	        }

	        _manifestBacklog.RemoveAll(i => i.RemainingAmount <= 0);
	    }

	    public bool TryHandleArrival(Ship ship)
	    {
	        return false;
	    }

	    public void HandleManifest(CargoManifest manifest)
	    {
	        _manifestBacklog.Add(manifest);
        }
	}
}