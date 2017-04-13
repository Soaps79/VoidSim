using Assets.WorldMaterials;

// some Station owned object(Station) holds a reference to this
namespace Assets.Placeables.Nodes
{
    public class ProductFactory : PlaceableNode
    {
        // Station takes this value and supplies factory with recipe list?
        public string ContainerType;
        private AutomatedContainer _container;

        public override void BroadcastPlacement()
        {
            // queue message FactoryPlaced
        }

        public override void Initialize()
        {
            // create child automated container
        }

        public void BeginCrafting() { } // pass on to container
    }
}
