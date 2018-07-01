using System.Linq;
using Assets.Logistics.Ships;
using Assets.WorldMaterials;

namespace Assets.Logistics.Transit
{
    // Object that represents the cargo concerns of any given vessel
    // Originally created for ships, but should keep generic carrier usage in mind when expanding
    public class CargoCarrier
    {
        // try removing Ship's knowledge of these
        private ProductInventory _inventory;
        public CargoManifestBook ManifestBook { get; private set; }
        public ShipNavigation Navigation { get; private set; }

        public void Initialize(ProductInventory inventory, ShipNavigation navigation, CargoManifestBook manifestBook)
        {
            _inventory = inventory;
            Navigation = navigation;
            ManifestBook = manifestBook;
        }

        public bool IsEmpty()
        {
            return _inventory.CurrentTotalCount <= 0;
        }

        // return if the carrier will have room after transferring at its next stop
        public int CanPickupProductThisStop(int productId)
        {
            var nextDelivery = ManifestBook.GetBuyerManifests(Navigation.CurrentDestination)
                .Where(i => i.ProductId == productId).Sum(j => j.RemainingAmount);

            var remainingPickup = ManifestBook.GetSellerManifests(Navigation.CurrentDestination)
                .Sum(j => j.RemainingAmount);

            return _inventory.GetProductRemainingSpace(productId) + nextDelivery - remainingPickup;
        }
    }
}