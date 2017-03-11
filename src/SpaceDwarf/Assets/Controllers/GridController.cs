using System;
using Assets.Model;
using QGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Controllers
{
    public class GridController : QScript
    {
        private Grid _grid;
        public int Width = 50;
        public int Height = 50;

        public Sprite SpaceTile;
        public Sprite FloorTile;
        public Sprite ScaffoldTile;
    
        private float _elapsed;

        void Start()
        {
            // Create grid model
            _grid = new Grid(name, Width, Height);

            // create game object for each tile
            CreateTiles();

            // hook debug randomization
            OnEveryUpdate += UpdateRandomTiles;
        }

        private void CreateTiles()
        {
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

                    tileData.RegisterOnTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGo); });
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

        private void UpdateRandomTiles(float deltaTime)
        {
            _elapsed += deltaTime;
            if (_elapsed > 2)
            {
                _elapsed = 0;
                RandomizeTiles();
            }
        }
        private void RandomizeTiles()
        {
            for (var i = 0; i < _grid.Width; i++)
            {
                for (var j = 0; j < _grid.Height; j++)
                {
                    var tileData = _grid.GetTileAt(i, j);
                    tileData.Type = (TileType) Random.Range(0, 3);
                }
            }
        }
    }
}
