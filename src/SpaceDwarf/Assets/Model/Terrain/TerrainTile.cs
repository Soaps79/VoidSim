using System;

namespace Assets.Model.Terrain
{
    public class TerrainTile : TileBase
    {
        private TerrainType _type;
        private Action<TerrainTile> _onTileTypeChanged;

        public TerrainType Type
        {
            get {  return _type;}
            set
            {
                if (_type != value)
                {
                    _type = value;
                    if (_onTileTypeChanged != null)
                        _onTileTypeChanged(this);
                }
            }
        }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }

        public TerrainTile(int x, int y, TerrainType terrainType)
            : base(x, y)
        {
            _type = terrainType;
        }

        public void RegisterOnTileTypeChangedCallback(Action<TerrainTile> callback)
        {
            _onTileTypeChanged += callback;
        }
    }
}