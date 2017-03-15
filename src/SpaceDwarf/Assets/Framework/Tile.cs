﻿namespace Assets.Framework
{
    /// <summary>
    /// Indexable generic tile
    /// </summary>
    public class Tile
    {
        private readonly int _x;
        private readonly int _y;

        public virtual string Name { get { return "Tile"; } }

        /// <summary>
        /// X component. Always positive.
        /// </summary>
        public int X { get { return _x; } }

        /// <summary>
        /// Y component. Always positive.
        /// </summary>
        public int Y { get { return _y; } }

        public Tile(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1},{2})", Name, _x, _y);
        }
    }
}