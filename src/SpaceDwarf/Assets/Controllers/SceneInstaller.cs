using Assets.View;
using Zenject;

namespace Assets.Controllers
{
    /// <summary>
    /// Add to scene to install dependencies.
    /// Hook to SceneContext item, created from Zenject menu under "Create" in hierarchy.
    /// </summary>
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        public override void InstallBindings()
        {
            // TerrainController receives injections
            Container.Bind<TerrainController>().AsSingle();

            // Satisfy ITileFactory dependency, from a new instance, and use this one everywhere (single instance)
            Container.Bind<ITileFactory>().FromInstance(new TileFactory()).AsSingle();

        }
    }
}
