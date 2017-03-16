using Assets.Framework;

namespace Assets.Model.Terrain
{
    /// <summary>
    /// Contains a chunk of terrain represented as a <see cref="Grid{T}"/> of <see cref="TerrainTile"/>
    /// </summary>
    public class TerrainRegion : Grid<TerrainTile>
    {
        // todo: move to config
        private const int RegionSize = 64;

        private readonly int _x;
        private readonly int _y;

        /// <summary>
        /// X component in the world. Can be negative.
        /// </summary>
        public int X { get { return _x; } }

        /// <summary>
        /// Y component in the world. Can be negative.
        /// </summary>
        public int Y { get { return _y; } }

        public override string Name { get { return "TerrainRegion"; } }
        
        public TerrainRegion(int x, int y)
            : base(RegionSize, RegionSize)
        {
            _x = x;
            _y = y;
            Initialize();
        }

        public TerrainTile GetTileAt(int x, int y)
        {
            return GetObjectAt(x, y);
        }

        public override string ToString()
        {
            var gridString = base.ToString();
            return string.Format("{0}({1},{2}) - {3}", Name, _x, _y, gridString);
        }

        private void Initialize()
        {
            var typeGrid = LoadRegion(_x, _y);
            InitializeTiles(typeGrid);
        }

        private void InitializeTiles(TerrainType[,] typeGrid)
        {
            for (var i = 0; i < RegionSize; i++)
            {
                for (var j = 0; j < RegionSize; j++)
                {
                    SetObjectAt(i, j, new TerrainTile(i, j, typeGrid[i, j]));
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
    }
}
