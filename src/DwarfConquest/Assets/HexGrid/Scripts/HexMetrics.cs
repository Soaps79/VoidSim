﻿using UnityEngine;

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
        public const float StreamBedElevationOffset = -1.75f;

        public const float WaterElevationOffset = -0.5f;

        // shores
        public const float WaterFactor = 0.6f;

        public const float WaterBlendFactor = 1f - WaterFactor;

        // features
        private static readonly float[][] FeatureThresholds =
        {
            // least likely <-> most likely
            new [] {0.0f, 0.0f, 0.4f},
            new [] {0.0f, 0.4f, 0.6f},
            new [] {0.4f, 0.6f, 0.8f}
        };

        public static float[] GetFeatureThresholds(int level)
        {
            return FeatureThresholds[level];
        }

        // walls
        public const float WallHeight = 4f;

        public const float WallYOffset = -1f;
        public const float WallThickness = 0.75f;
        public const float WallElevationOffset = VerticalTerraceStepSize;
        public const float WallTowerThreshold = 0.7f;

        public static Vector3 WallThicknessOffset(Vector3 near, Vector3 far)
        {
            Vector3 offset;
            offset.x = far.x - near.x;
            offset.y = 0f;
            offset.z = far.z - near.z;
            return offset.normalized * (WallThickness * 0.5f);
        }

        public const int HashGridSize = 256;
        public const float HashGridScale = 0.25f;

        private static HexHash[] _hashGrid;

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

        public static void InitializeHashGrid(int seed)
        {
            _hashGrid = new HexHash[HashGridSize * HashGridSize];
            var currentState = Random.state;
            Random.InitState(seed);
            for (var i = 0; i < _hashGrid.Length; i++)
            {
                _hashGrid[i] = HexHash.Create();
            }
            Random.state = currentState;
        }

        public static HexHash SampleHashGrid(Vector3 position)
        {
            var x = (int)(position.x * HashGridScale) % HashGridSize;
            if (x < 0) { x += HashGridSize; }

            var z = (int)(position.z * HashGridScale) % HashGridSize;
            if (z < 0) { z += HashGridSize; }

            return _hashGrid[x + z * HashGridSize];
        }

        public static Vector4 SampleNoise(Vector3 position)
        {
            return NoiseSource.GetPixelBilinear(
                position.x * NoiseScale,
                position.z * NoiseScale);
        }

        // water
        public static Vector3 GetFirstWaterCorner(HexDirection direction)
        {
            return Corners[(int)direction] * WaterFactor;
        }

        public static Vector3 GetSecondWaterCorner(HexDirection direction)
        {
            return Corners[(int)direction + 1] * WaterFactor;
        }

        public static Vector3 GetWaterBridge(HexDirection direction)
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) *
                   WaterBlendFactor;
        }

        // land
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

        // edges
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
        }

        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            var h = step * HorizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            // ReSharper disable once PossibleLossOfFraction
            // reason: integer division
            var v = ((step + 1) / 2) * VerticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        public static Color TerraceLerp(Color a, Color b, int step)
        {
            var h = step * HorizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        public static Vector3 WallLerp(Vector3 near, Vector3 far)
        {
            near.x += (far.x - near.x) * 0.5f;
            near.z += (far.z - near.z) * 0.5f;
            var v = near.y < far.y ? WallElevationOffset : (1f - WallElevationOffset);
            near.y += (far.y - near.y) * v + WallYOffset;
            return near;
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

        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) *
                   (0.5f * SolidFactor);
        }

        // utility
        public static Vector3 Perturb(Vector3 position)
        {
            var sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * CellPerturbStrength;
            position.z += (sample.z * 2f - 1f) * CellPerturbStrength;
            return position;
        }
    }
}