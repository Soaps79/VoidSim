using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public static class HexMetrics
    {
        /// <summary>
        /// Extent of hexagon
        /// </summary>
        public const float OuterRadius = 10f;

        // given edge length e, inner radius is
        // sqrt(e^2 - (e/2)^2) == e * sqrt(3) / 2 ~= 0.886e
        private const float EdgeLengthRatio = 0.866025404f;

        /// <summary>
        /// Inscribed tangent circle radius
        /// </summary>
        public const float InnerRadius = EdgeLengthRatio * OuterRadius;

        // color blending zones
        public const float SolidFactor = 0.75f;
        public const float BlendFactor = 1f - SolidFactor;
        
        private static readonly Vector3[] Corners =
        {
            new Vector3(0f, 0f, OuterRadius),
            new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
            new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(0f, 0f, -OuterRadius),
            new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
            // final corner coincident of first corner
            new Vector3(0f, 0f, OuterRadius)
        };

        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return Corners[(int)direction];
        }

        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return Corners[(int)direction + 1];
        }

        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return Corners[(int)direction] * SolidFactor;
        }

        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return Corners[(int)direction + 1] * SolidFactor;
        }

        public static Vector3 GetBridge(HexDirection direction)
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
        }
    }
}