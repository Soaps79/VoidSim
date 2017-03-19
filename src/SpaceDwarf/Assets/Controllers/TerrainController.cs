using System;
using Assets.Controllers.GUI;
using Assets.Framework;
using Assets.Model.Terrain;
using Assets.View;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Controllers
{
    public class TerrainController : OrderedEventBehavior
    {
        [Tooltip("View for Terrain.")]
        public TerrainView View;
        
        public TerrainWorld World { get; private set; }

        [Inject]
        public ITileFactory TileFactory;
        
        protected override void OnStart ()
        {
            World = new TerrainWorld();

            // hook actions
            World.RegisterOnRegionAddedCallback((world, tile) => { OnTerrainRegionAdded(world, tile, gameObject);});

            // initialize regions (after hooking!)
            // todo: load based on player position
            World.InitializeRegions();
        }

        public void OnTerrainRegionAdded(TerrainWorld world, TerrainRegion region, GameObject worldGo)
        {
            // create game object for region
            var regionGo = new GameObject(region.Name);
            regionGo.transform.parent = worldGo.transform;

            // hook any region events

            // create game objects for tiles of region
            for (var i = 0; i < region.Width; i++)
            {
                for (var j = 0; j < region.Height; j++)
                {
                    // query region for tile data
                    var tile = region.GetTileAt(i, j);
                    var tileGo = TileFactory.CreateTerrainTile(tile, View, i, j, regionGo);

                    // hook tile events
                    tile.RegisterOnTileTypeChangedCallback((t) => { OnTileTypeChanged(t, tileGo); });
                }
            }

            // translate the entire region to center
            regionGo.transform.Translate(new Vector3(-0.5f * region.Width, -0.5f * region.Height, 0f));

            //todo: translate to TerrainWorldView
        }

        private void OnTileTypeChanged(TerrainTile tile, GameObject tileGo)
        {
            tileGo.GetComponent<SpriteRenderer>().sprite = TileFactory.AssignTileSprite(tile.Type, View);
        }

        

        
    }
}
