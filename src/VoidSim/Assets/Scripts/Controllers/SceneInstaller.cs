using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldMaterials;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public WorldClock WorldClock;
    public Station Station;
    public ProductLookup ProductLookup;


    public override void InstallBindings()
    {
        // Core game objects
        Container.Bind<WorldClock>().FromInstance(WorldClock).AsSingle();
        Container.Bind<ProductLookup>().FromInstance(ProductLookup).AsSingle();
        Container.Bind<Station>().FromInstance(Station).AsSingle();
        Container.BindFactory<Inventory, Inventory.Factory>();
    }
}
