using Assets.Controllers;
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
        public override void InstallBindings()
        {
            Container.Bind<Vector2>().ToSelf().FromInstance(new Vector2(0, 0)).WhenInjectedInto<PlayerCharacter>();
            Container.Bind<PlayerCharacter>().AsSingle();

            // controllers from the scene
            Container.Bind<TerrainController>().FromInstance(TerrainController.Instance).AsSingle();
            Container.Bind<PlayerController>().FromInstance(PlayerController.Instance).AsSingle();
            Container.Bind<CameraController>().FromInstance(CameraController.Instance).AsSingle();
            
            // Satisfy ITileFactory dependency, from a new instance, and use this one everywhere (single instance)
            var tileFactory = new TileFactory();
            Container.Bind<ITileFactory>().To<TileFactory>().FromInstance(tileFactory).AsSingle();
        }
    }
}
