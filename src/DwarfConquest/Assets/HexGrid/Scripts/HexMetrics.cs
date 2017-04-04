using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public static class HexMetrics
    {
        /// <summary>
        /// Extent of hexagon
        /// </summary>
        public const float OuterRadius = 10f;
        public const float InnterRadius = OuterRadius * OuterToInner;

        // given edge length e, inner radius is
        // sqrt(e^2 - (e/2)^2) == e * sqrt(3) / 2 ~= 0.886e
        private const float EdgeLengthRatio = 0.866025404f;
        public const float OuterToInner = EdgeLengthRatio;
        public const float InnerToOuter = 1f / OuterToInner;

        /// <summary>
        /// Inscribed tangent circle radius
        /// </summary>
        public const float InnerRadius = EdgeLengthRatio * OuterRadius;

        // color blending zones
        public const float SolidFactor = 0.8f;
        public const float BlendFactor = 1f - SolidFactor;

        // elevation and terraces
        public const float ElevationStep = 3f;

        public const int TerracesPerSlope = 2;
        public const int TerraceSteps = TerracesPerSlope * 2 + 1;

        public const float HorizontalTerraceStepSize = 1f / TerraceSteps;
        public const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

        // noise
        public static Texture2D NoiseSource;
        public const float NoiseScale = 0.003f;

        public const float CellPerturbStrength = 4f;
        public const float ElevationPerturbStrength = 1.5f;

        // chunks
        public const int ChunkSizeX = 5;
        public const int ChunkSizeZ = 5;

        // rivers
        public const float StreamBedElevationOffset = -1f;

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

        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            //var h = step * HorizontalTerraceStepSize;
            //a.x += (b.x - a.x) * h;
            //a.z += (b.z - a.z) * h;

            //// ReSharper disable once PossibleLossOfFraction
            //// reason: integer division
            //var v = ((step + 1) / 2) * VerticalTerraceStepSize;
            //a.y += (b.y - a.y) * v;

            //return a;

            float h = step * HexMetrics.HorizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            // ReSharper disable once PossibleLossOfFraction
            float v = ((step + 1) / 2) * HexMetrics.VerticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        public static Color TerraceLerp(Color a, Color b, int step)
        {
            var h = step * HorizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }

            var delta = elevation2 - elevation1;
            if (Mathf.Abs(delta) <= 1)
            {
                return HexEdgeType.Slope;
            }

            return HexEdgeType.Cliff;
        }

        public static Vector4 SampleNoise(Vector3 position)
        {
            return NoiseSource.GetPixelBilinear(
                position.x * NoiseScale, 
                position.z * NoiseScale);
        }

        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (Corners[(int) direction] + Corners[(int) direction + 1]) *
                   (0.5f * SolidFactor);
        }
    }
}