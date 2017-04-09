using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
using Zenject;

namespace Assets.Controllers
{
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        public WorldClock WorldClock;
        public Station.Station Station;
        public ProductLookup ProductLookup;


        public override void InstallBindings()
        {
            // Core game objects
            Container.Bind<WorldClock>().FromInstance(WorldClock).AsSingle();
            Container.Bind<ProductLookup>().FromInstance(ProductLookup).AsSingle();
            Container.Bind<Station.Station>().FromInstance(Station).AsSingle();
            Container.BindFactory<Inventory, Inventory.Factory>();
        }
    }
}
