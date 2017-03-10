using System;
using UnityEngine;

namespace Assets.Scripts
{
    public enum TileType { Space=0, Scaffolding=1, Floor=2 }

    public class Tile
    {
        private readonly Grid _owner;

        private readonly string _name;
        private readonly int _x;
        private readonly int _y;
        private TileType _tileType = TileType.Space;

        private Action<Tile> _onTileTypeChanged;

        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public string Name { get { return _name; } }

        public TileType Type
        {
            get { return _tileType; }
            set
            {
                if (_tileType != value)
                {
                    _tileType = value;
                    if (_onTileTypeChanged != null)
                        _onTileTypeChanged(this);
                }
            }
        }

        public Tile(Grid owner, int x, int y)
        {
            _owner = owner;
            _x = x;
            _y = y;
            _name = string.Format("{0}-Tile:({1}, {2})", _owner.Name, _x, _y);
        }

        public bool CanBuild(TileType type)
        {
            if (type == TileType.Space)
            {
                return true;
            }

            if (type == TileType.Scaffolding)
            {
                return _tileType == TileType.Space
                    || _tileType == TileType.Floor;
            }

            if (type == TileType.Floor)
            {
                return _tileType == TileType.Scaffolding;
            }

            Debug.LogError("CanBuild - Unknown TileType");
            return false;
        }

        public void RegisterOnTileTypeChangedCallback(Action<Tile> callback)
        {
            _onTileTypeChanged += callback;
        }
    }
}
