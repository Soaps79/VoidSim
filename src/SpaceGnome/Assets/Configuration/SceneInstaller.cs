using Assets.Model.Terrain;
using Zenject;

namespace Assets.Configuration
{
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        public override void InstallBindings()
        {
            var terrainFactory = new TerrainFactory();
            Container.Bind<TerrainFactory>().FromInstance(terrainFactory).AsSingle();
        }
    }
}
