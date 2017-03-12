using System;
using Assets.Model;
using QGame;
using UnityEngine;
using UnityEngine.WSA;

namespace Assets.Controllers
{
    public class TerrainController : QScript
    {
        public Sprite GreenGrassTile0;
        public Sprite GreenGrassTile1;
        public Sprite GreenGrassTile2;
        public Sprite GreenGrassTile3;

        private TerrainWorld _world;
        
        void Start ()
        {
            _world = new TerrainWorld();

            // hook actions
            _world.RegisterOnRegionAddedCallback((world, tile) => { OnTerrainRegionAdded(world, tile, gameObject);});

            // initialize regions (after hooking!)
            // todo: load based on player position
            _world.InitializeRegions();
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
                    var tileGo = new GameObject(tile.Name);

                    // update position relative to region parent
                    tileGo.transform.position = new Vector3(i, j, 0);
                    tileGo.transform.parent = regionGo.transform;

                    // create sprite component and assign texture
                    var spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = AssignTileSprite(tile.Type);
                    spriteRenderer.sortingLayerName = "Terrain";

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
            tileGo.GetComponent<SpriteRenderer>().sprite = AssignTileSprite(tile.Type);
        }

        private Sprite AssignTileSprite(TerrainType tileType)
        {
            switch (tileType)
            {
                case TerrainType.GreenGrass:
                    return GetGreenGrassSprite();
                default:
                    throw new ArgumentOutOfRangeException("tileType", tileType, null);
            }
        }

        private Sprite GetGreenGrassSprite()
        {
            var type = UnityEngine.Random.Range(0, 4);
            switch (type)
            {
                case 0:
                    return GreenGrassTile0;
                case 1:
                    return GreenGrassTile1;
                case 2:
                    return GreenGrassTile2;
                case 3:
                    return GreenGrassTile3;
                default:
                    throw new IndexOutOfRangeException("Invalid GreenGrassTile index.");

            }
        }
    }
}
