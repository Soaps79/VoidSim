using System;
using QGame;
using UnityEngine;

namespace Assets.Scripts
{
    public class GridController : QScript
    {
        private Grid _grid;
        public int Width = 50;
        public int Height = 50;

        public Sprite SpaceTile;
        public Sprite FloorTile;
        public Sprite ScaffoldTile;
        

        void Start()
        {
            // Create grid model
            _grid = new Grid(name, Width, Height);

            // create game object for each tile
            for (var i = 0; i < _grid.Width; i++)
            {
                for (var j = 0; j < _grid.Height; j++)
                {
                    var tileData = _grid.GetTileAt(i, j);
                    var tileGo = new GameObject(tileData.Name);
                    tileGo.transform.position = new Vector3(i, j, 0);
                    tileGo.transform.parent = transform;

                    var spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = AssignTileSprite(tileData.Type);
                }
            }
        }

        private Sprite AssignTileSprite(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Space:
                    return SpaceTile;
                case TileType.Scaffolding:
                    return ScaffoldTile;
                case TileType.Floor:
                    return FloorTile;
                default:
                    throw new ArgumentOutOfRangeException("tileType", tileType, null);
            }
        }

        public void OnTileTypeChanged(Tile tileData, GameObject tileGo)
        {
            tileGo.GetComponent<SpriteRenderer>().sprite = AssignTileSprite(tileData.Type);
        }
    }
}
