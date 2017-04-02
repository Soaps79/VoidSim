using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates Coordinates;
        public Color Color;
        public RectTransform UiRect;

        private int _elevation;

        [SerializeField]
        public HexCell[] Neighbors;

        public int Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;
                var position = transform.localPosition;
                position.y = value * HexMetrics.ElevationStep;
                transform.localPosition = position;

                var uiPosition = UiRect.localPosition;
                uiPosition.z = _elevation * -HexMetrics.ElevationStep;
                UiRect.localPosition = uiPosition;
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
    }
}