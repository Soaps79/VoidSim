using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Assets.Model
{
    /// <summary>
    /// Contains a chunk of terrain
    /// </summary>
    public class TerrainRegion
    {
        // todo: move to config
        private const int RegionSize = 64;

        private readonly int _x;
        private readonly int _y;
        private readonly TerrainTile[,] _tiles = new TerrainTile[RegionSize, RegionSize];

        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public int Width { get { return RegionSize; } }
        public int Height { get { return RegionSize; } }

        public string Name { get { return string.Format("Region ({0},{1})", _x, _y); } }

        public TerrainRegion(int x, int y)
        {
            _x = x;
            _y = y;

            Initialize(x, y);
        }

        private void Initialize(int x, int y)
        {
            
            var typeGrid = LoadRegion(x, y);
            InitializeTiles(typeGrid);
        }

        private void InitializeTiles(TerrainType[,] typeGrid)
        {
            for (var i = 0; i < RegionSize; i++)
            {
                for (var j = 0; j < RegionSize; j++)
                {
                    _tiles[i, j] = new TerrainTile(i, j, typeGrid[i, j]);
                }
            }
        }

        private TerrainType[,] LoadRegion(int x, int y)
        {
            // todo: read from map file
            var types = new TerrainType[RegionSize, RegionSize];
            for (var i = 0; i < RegionSize; i++)
            {
                for (var j = 0; j < RegionSize; j++)
                {
                    types[i, j] = TerrainType.GreenGrass;
                }
            }
            return types;
        }

        public TerrainTile GetTileAt(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
