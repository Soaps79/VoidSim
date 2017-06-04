using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.Void;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Zenject;

namespace Assets.Controllers
{
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        public WorldClock WorldClock;
        public Station.Station Station;
        public ProductLookup ProductLookup;
        public VoidActor Void;


        public override void InstallBindings()
        {
            // Core game objects
            Container.Bind<WorldClock>().FromInstance(WorldClock).AsSingle();
            Container.Bind<ProductLookup>().FromInstance(ProductLookup).AsSingle();
            Container.Bind<Station.Station>().FromInstance(Station).AsSingle();
            Container.Bind<VoidActor>().FromInstance(Void).AsSingle();
            Container.BindFactory<Inventory, Inventory.Factory>();
        }
    }
}
