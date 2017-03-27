using Assets.Model.Terrain;
using Assets.View;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Controllers.Terrain
{
    public class TerrainController : SingletonBehavior<TerrainController>
    {
        [Tooltip("View for Terrain.")]
        public TerrainView View;

        [Tooltip("Root for Terrain objects.")]
        public Transform TerrainRoot;
        
        public TerrainWorld World { get; private set; }

        public float TerrainViewOffset;

        [Inject]
        public ITileFactory TileFactory;
        
        protected override void OnStart ()
        {
            base.OnStart();

            TerrainViewOffset = -0.5f * TerrainRegion.RegionSize;
             
            Debug.Log("OnStart");
            World = new TerrainWorld();

            // hook actions
            World.RegisterOnRegionAddedCallback((world, tile) => { OnTerrainRegionAdded(world, tile);});

            // initialize regions (after hooking!)
            // todo: load based on player position
            World.InitializeRegions();
        }

        public void OnTerrainRegionAdded(TerrainWorld world, TerrainRegion region)
        {
            // create game object for region
            var regionGo = new GameObject(region.Name);
            regionGo.transform.parent = TerrainRoot;
            regionGo.transform.localPosition = new Vector3(0, 0, 0);

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
            regionGo.transform.Translate(new Vector3(TerrainViewOffset, TerrainViewOffset, 0f));

            //todo: translate to TerrainWorldView
        }

        private void OnTileTypeChanged(TerrainTile tile, GameObject tileGo)
        {
            tileGo.GetComponent<SpriteRenderer>().sprite = TileFactory.AssignTileSprite(tile.Type, View);
        }

        

        
    }
}
