using System;

namespace Assets.Model
{
    public class TerrainTile
    {
        private TerrainType _type;
        private readonly int _x;
        private readonly int _y;

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

        public string Name { get { return string.Format("Tile ({0}, {1}) - {2}", _x, _y, _type); } }

        public TerrainTile(int x, int y, TerrainType terrainType)
        {
            _x = x;
            _y = y;
            _type = terrainType;
        }

        public void RegisterOnTileTypeChangedCallback(Action<TerrainTile> callback)
        {
            _onTileTypeChanged += callback;
        }
    }
}