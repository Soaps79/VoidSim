using System;
using Assets.Framework;

namespace Assets.Model.Terrain
{
    /// <summary>
    /// Typed <see cref="Tile"/> for use in Terrain.
    /// </summary>
    public class TerrainTile : Tile
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
        
        public TerrainTile(int x, int y, TerrainType terrainType)
            : base(x, y)
        {
            _type = terrainType;
        }

        public void RegisterOnTileTypeChangedCallback(Action<TerrainTile> callback)
        {
            _onTileTypeChanged += callback;
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            return string.Format("{0} Terrain ({1})", baseStr, _type);
        }
    }
}