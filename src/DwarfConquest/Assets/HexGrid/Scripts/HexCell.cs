using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates Coordinates;
        private Color _color;
        public RectTransform UiRect;
        public HexGridChunk Chunk;

        private int _elevation = int.MinValue;

        [SerializeField]
        public HexCell[] Neighbors;

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

                Refresh();
            }
        }

        public Vector3 Position {  get { return transform.localPosition; } }

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