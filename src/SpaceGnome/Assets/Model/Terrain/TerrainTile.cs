using System;

namespace Assets.Model.Terrain
{
    /// <summary>
    /// Typed <see cref="Tile"/> for use in Terrain.
    /// </summary>
    public class TerrainTile : Tile
    {
        private TerrainType _type;
        public Action<TerrainTile> OnTileTypeChanged { get; set; }

        public TerrainTile(int x, int y, TerrainType type)
            : base(x, y)
        {
            _type = type;
        }

        public TerrainType Type
        {
            get { return _type; }
            set
            {
                if (_type == value) { return; }

                _type = value;
                if (OnTileTypeChanged != null)
                    OnTileTypeChanged(this);
            }
        }

    }
}
