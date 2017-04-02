using System;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    [Serializable]
    public class HexCoordinates
    {
        [SerializeField] private int _x, _z;

        public int X
        {
            get { return _x; }
            private set { _x = value; }
        }

        public int Z
        {
            get { return _z; }
            private set { _z = value; }
        }

        public int Y { get { return -X - Z; } }

        public HexCoordinates(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            var x = position.x / (HexMetrics.InnerRadius * 2f);
            var y = -x;

            var offset = position.z / (HexMetrics.OuterRadius * 3f);
            x -= offset;
            y -= offset;

            var iX = Mathf.RoundToInt(x);
            var iY = Mathf.RoundToInt(y);
            var iZ = Mathf.RoundToInt(-x - y);

            if (iX + iY + iZ == 0)
            {
                return new HexCoordinates(iX, iZ);
            }

            // compensate for rounding error
            var dX = Mathf.Abs(x - iX);
            var dY = Mathf.Abs(y - iY);
            var dZ = Mathf.Abs(-x - y - iZ);

            // reconstruct using the least rounded of x and z
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }

            return new HexCoordinates(iX, iZ);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }

        public string ToStringOnSeparateLines()
        {
            return string.Format("{0}\n{1}\n{2}", X, Y, Z);
        }
    }
}