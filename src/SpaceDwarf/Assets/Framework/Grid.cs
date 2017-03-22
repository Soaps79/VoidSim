using System;
using UnityEngine;


// todo: move to QGame
namespace Assets.Framework
{
    /// <summary>
    /// Logical grid of objects
    /// </summary>
    /// <typeparam name="T">Type of objects in the grid</typeparam>
    public class Grid<T>
    {
        private readonly int _width;
        private readonly int _height;

        private readonly T[,] _gridObjects;

        private Action<T, T> _onSetTile;

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;

            _gridObjects = new T[_width, _height];
        }

        public virtual string Name
        {
            get { return string.Format("Grid<{0}>", typeof(T).Name); }
        }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public T GetObjectAt(int x, int y)
        {
            if (IsIndexOutOfBounds(x, y))
                return default(T);
            
            return _gridObjects[x, y];
        }

        public T SetObjectAt(int x, int y, T tile)
        {
            if (IsIndexOutOfBounds(x, y))
                return default(T);

            var oldTile = GetObjectAt(x, y);
            _gridObjects[x, y] = tile;

            // fire set event
            if (_onSetTile != null)
                _onSetTile(oldTile, tile);

            return oldTile;
        }

        public void RegisterOnSetObjectCallback(Action<T, T> callback)
        {
            _onSetTile += callback;
        }

        public override string ToString()
        {
            return string.Format("{0} width({1}), height({2})", Name, _width, _height);
        }

        private bool IsIndexOutOfBounds(int x, int y)
        {
            var outOfBounds = false;

            if (x >= _width || x < 0)
            {
                Debug.LogWarningFormat("Grid: x index out of range ({0}), max ({1})", new[] { x, _width });
                outOfBounds = true;
            }

            if (y >= _height || y < 0)
            {
                Debug.LogWarningFormat("Grid: y index out of range ({0}), max ({1})", new[] { y, _height });
                outOfBounds = true;
            }

            return outOfBounds;
        }
    }
}
