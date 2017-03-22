using System;
using UnityEngine;

namespace QGame.Common
{
    /// <summary>
    /// Logical grid of objects
    /// </summary>
    /// <typeparam name="T">Type of objects in the grid</typeparam>
    public class Grid<T>
    {
        private readonly T[,] _gridObjects;

        public Action<T, T> OnSetElement;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            _gridObjects = new T[width, height];
        }

        public virtual string Name
        {
            get { return string.Format("Grid<{0}>", typeof(T).Name); }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public T GetObjectAt(int x, int y)
        {
            if (IsIndexOutOfBounds(x, y))
                return default(T);

            return _gridObjects[x, y];
        }

        public T SetElementAt(int x, int y, T tile)
        {
            if (IsIndexOutOfBounds(x, y))
                return default(T);

            var oldTile = GetObjectAt(x, y);
            _gridObjects[x, y] = tile;

            // fire set event
            if (OnSetElement != null)
                OnSetElement(oldTile, tile);

            return oldTile;
        }

        public override string ToString()
        {
            return string.Format("{0} width({1}), height({2})", Name, Width, Height);
        }

        private bool IsIndexOutOfBounds(int x, int y)
        {
            var outOfBounds = false;

            if (x >= Width || x < 0)
            {
                Debug.LogWarning(string.Format("Grid: x index out of range ({0}), max ({1})", x, Width));
                outOfBounds = true;
            }

            if (y >= Height || y < 0)
            {
                Debug.LogWarning(string.Format("Grid: y index out of range ({0}), max ({1})", y, Height));
                outOfBounds = true;
            }

            return outOfBounds;
        }
    }
}
