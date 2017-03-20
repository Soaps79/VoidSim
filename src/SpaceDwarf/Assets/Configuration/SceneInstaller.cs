using Assets.Controllers;
using Assets.Controllers.GameStates;
using Assets.Model;
using Assets.View;
using UnityEngine;
using Zenject;

namespace Assets.Configuration
{
    /// <summary>
    /// Add to scene to install dependencies.
    /// Hook to SceneContext item, created from Zenject menu under "Create" in hierarchy.
    /// </summary>
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        public CameraSettings CameraSettings;

        public Material SelectionMaterial;

        public override void InstallBindings()
        {
            Container.Bind<Vector2>().ToSelf().FromInstance(new Vector2(0, 0)).WhenInjectedInto<PlayerCharacter>();
            Container.Bind<PlayerCharacter>().AsSingle();

            // controllers from the scene
            Container.Bind<GameStateController>().FromInstance(GameStateController.Instance).AsSingle();
            Container.Bind<TerrainController>().FromInstance(TerrainController.Instance).AsSingle();
            Container.Bind<PlayerController>().FromInstance(PlayerController.Instance).AsSingle();
            Container.Bind<CameraController>().FromInstance(CameraController.Instance).AsSingle();

            // config settings
            Container.Bind<CameraSettings>().FromInstance(CameraSettings).AsSingle();

            // Satisfy ITileFactory dependency, from a new instance, and use this one everywhere (single instance)
            var tileFactory = new TileFactory(SelectionMaterial);
            Container.Bind<ITileFactory>().To<TileFactory>().FromInstance(tileFactory).AsSingle();

            // game model
            Container.Bind<GameModel>().AsSingle();

            Container.Bind<Material>().ToSelf().FromInstance(SelectionMaterial).WhenInjectedInto<SelectionBehavior>();

            // bind states
            Container.Bind<PauseGameState>().AsSingle();
            Container.Bind<DefaultGlobalState>().AsSingle();

            Container.Bind<SelectionGameState>().AsSingle();
            Container.Bind<DefaultGameState>().AsSingle();
        }
    }
}
