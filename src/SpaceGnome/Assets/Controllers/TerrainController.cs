using System.Collections.Generic;
using Assets.Model.Terrain;
using Assets.Views.Terrain;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Controllers
{
    public class TerrainController : SingletonBehavior<TerrainController>
    {
        [Tooltip("Views for terrain tiles")]
        public List<TerrainTypeViewDetails> Views;

        [Tooltip("Root game object for Terrain.")]
        public Transform TerrainRoot;

        public TerrainWorld World { get { return TerrainWorld.Instance; } }

        [Inject] private TerrainFactory _factory;

        public readonly Dictionary<TerrainType, TerrainTypeViewDetails> ViewDictionary = new Dictionary<TerrainType, TerrainTypeViewDetails>();

        protected override void OnStart()
        {
            base.OnStart();

            // create handy dictionary of terrain view types
            foreach (var view in Views)
            {
                ViewDictionary.Add(view.Type, view);
            }

            World.OnRegionAdded += (world, tile) => OnTerrainRegionAdded(world, tile, TerrainRoot);
            World.InitializeRegions();
        }

        private void OnTerrainRegionAdded(TerrainWorld world, TerrainRegion region, Transform root)
        {
            // create a game object for the region
            var regionGo = _factory.CreateRegion(region.Name, root);

            for (var i = 0; i < region.Width; i++)
            {
                for (var j = 0; j < region.Height; j++)
                {
                    var tileData = region.GetTileAt(i, j);
                    var tileGo = _factory.CreateTile(tileData, Views, i, j, regionGo);

                    // hook tile events
                    tileData.OnTileTypeChanged += (t) => OnTileTypeChanged(t, tileGo);
                }
            }

            // todo: manage somewhere else
            // translate entire region so (0, 0) is at the center
            regionGo.transform.Translate(new Vector3(-0.5f * region.Width, -0.5f * region.Height));

        }

        private void OnTileTypeChanged(TerrainTile tile, GameObject tileGo)
        {
            var renderer = tileGo.GetComponent<SpriteRenderer>();
            renderer.sprite = _factory.AssignTileSprite(tile.Type, Views);
        }
    }
}
