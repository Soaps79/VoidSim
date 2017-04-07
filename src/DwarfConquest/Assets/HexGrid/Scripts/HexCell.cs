using System.Collections.Generic;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates Coordinates;
        
        public RectTransform UiRect;
        public HexGridChunk Chunk;

        [SerializeField]
        public HexCell[] Neighbors;

        private Color _color;

        private int _elevation = int.MinValue;
        
        public int Elevation
        {
            get { return _elevation; }
            set
            {
                if (_elevation == value)
                {
                    return;
                }

                _elevation = value;
                var position = transform.localPosition;
                position.y = value * HexMetrics.ElevationStep;
                position.y +=
                    (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;

                transform.localPosition = position;

                var uiPosition = UiRect.localPosition;
                uiPosition.z = -position.y;
                UiRect.localPosition = uiPosition;

                ValidateRivers();

                Refresh();
            }
        }

        public Vector3 Position { get { return transform.localPosition; } }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                {
                    return;
                }

                _color = value;
                Refresh();
            }
        }

        #region Water

        private int _waterLevel;

        public int WaterLevel
        {
            get { return _waterLevel; }
            set
            {
                if (_waterLevel == value)
                {
                    return;
                }
                _waterLevel = value;
                ValidateRivers();
                Refresh();
            }
        }

        public bool IsUnderwater { get { return _waterLevel > _elevation; } }

        public float WaterSurfaceY
        {
            get
            {
                return (WaterLevel + HexMetrics.WaterElevationOffset) *
                       HexMetrics.ElevationStep;
            }
        }

        #endregion

        #region Rivers

        public bool HasIncomingRiver { get; private set; }

        public HexDirection IncomingRiver { get; private set; }

        public bool HasOutgoingRiver { get; private set; }
        public HexDirection OutgoingRiver { get; private set; }

        public bool HasRiver { get { return HasIncomingRiver || HasOutgoingRiver; } }
        public bool HasRiverTerminus { get { return HasIncomingRiver != HasOutgoingRiver; } }

        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return HasIncomingRiver && IncomingRiver == direction ||
                   HasOutgoingRiver && OutgoingRiver == direction;
        }

        public void RemoveOutgoingRiver()
        {
            if (!HasOutgoingRiver)
            {
                return;
            }

            HasOutgoingRiver = false;
            RefreshSelfOnly();

            var neighbor = GetNeighbor(OutgoingRiver);
            neighbor.HasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveIncomingRiver()
        {
            if (!HasIncomingRiver)
            {
                return;
            }

            HasIncomingRiver = false;
            RefreshSelfOnly();

            var neighbor = GetNeighbor(IncomingRiver);
            neighbor.HasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        public float StreamBedY
        {
            get { return (Elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep; }
        }

        public float RiverSurfaceY
        {
            get
            {
                return (Elevation + HexMetrics.WaterElevationOffset) *
                       HexMetrics.ElevationStep;
            }
        }

        private bool IsValidRiverDestination(HexCell neighbor)
        {
            return neighbor &&
                   (Elevation >= neighbor.Elevation || WaterLevel == neighbor.Elevation);
        }

        public void SetOutgoingRiver(HexDirection direction)
        {
            if (HasOutgoingRiver && OutgoingRiver == direction)
            {
                // all set
                return;
            }

            var neighbor = GetNeighbor(direction);
            if (!IsValidRiverDestination(neighbor))
            {
                // can't flow into the void or up hill
                return;
            }

            RemoveOutgoingRiver();
            if (HasIncomingRiver && IncomingRiver == direction)
            {
                RemoveIncomingRiver();
            }

            HasOutgoingRiver = true;
            OutgoingRiver = direction;
            RefreshSelfOnly();

            neighbor.RemoveIncomingRiver();
            neighbor.HasIncomingRiver = true;
            neighbor.IncomingRiver = direction.Opposite();
            neighbor.RefreshSelfOnly();
        }

        private void ValidateRivers()
        {
            if (HasOutgoingRiver &&
                !IsValidRiverDestination(GetNeighbor(OutgoingRiver)))
            {
                RemoveOutgoingRiver();
            }

            if (HasIncomingRiver &&
                !GetNeighbor(IncomingRiver).IsValidRiverDestination(this))
            {
                RemoveIncomingRiver();
            }
        }

        #endregion

        #region Features

        private int _urbanLevel;
        private int _farmLevel;
        private int _plantLevel;

        public int UrbanLevel
        {
            get { return _urbanLevel; }
            set
            {
                if (_urbanLevel != value)
                {
                    _urbanLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        public int FarmLevel
        {
            get { return _farmLevel; }
            set
            {
                if (_farmLevel != value)
                {
                    _farmLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        public int PlantLevel
        {
            get { return _plantLevel; }
            set
            {
                if (_plantLevel != value)
                {
                    _plantLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        private bool _isWalled;

        public bool IsWalled
        {
            get { return _isWalled; }
            set
            {
                if (_isWalled != value)
                {
                    _isWalled = value;
                    Refresh();
                }
            }
        }

        #endregion  

        public HexCell GetNeighbor(HexDirection direction)
        {
            return Neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            Neighbors[(int)direction] = cell;
            cell.Neighbors[(int)direction.Opposite()] = this;
        }

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(
                _elevation, GetNeighbor(direction).Elevation);
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
        }

        private void RefreshSelfOnly()
        {
            if (Chunk != null)
            {
                Chunk.Refresh();
            }
        }

        private void Refresh()
        {
            if (Chunk != null)
            {
                Chunk.Refresh();
                for (var i = 0; i < Neighbors.Length; i++)
                {
                    var neighbor = Neighbors[i];
                    if (neighbor != null && neighbor.Chunk != Chunk)
                    {
                        neighbor.Chunk.Refresh();
                    }
                }
            }
        }
    }
}