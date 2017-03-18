using Assets.Controllers.GameStates;
using Assets.Model;
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
        public TerrainController TerrainController;
        public PlayerController PlayerController;

        public override void InstallBindings()
        {
            // controllers from the scene
            Container.Bind<TerrainController>().FromInstance(TerrainController).AsSingle();
            Container.Bind<PlayerController>().FromInstance(PlayerController).AsSingle();

            // Satisfy ITileFactory dependency, from a new instance, and use this one everywhere (single instance)
            Container.Bind<ITileFactory>().FromInstance(new TileFactory()).AsSingle();

            // game model
            Container.Bind<GameModel>().AsSingle();

            // bind states
            Container.Bind<PauseGameState>().AsSingle();
            Container.Bind<DefaultGlobalState>().AsSingle();
            Container.Bind<DefaultGameState>().AsSingle();
        }
    }
}
