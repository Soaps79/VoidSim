using UnityEngine;
using UnityEngine.UI;

namespace Assets.HexGrid.Scripts
{
    public class HexGrid : MonoBehaviour
    {
        public int Width = 6;
        public int Height = 6;

        public Color DefaultColor = Color.white;

        public HexCell CellPrefab;
        public Text CellLabelPrefab;

        private HexCell[] _cells;
        private Canvas _gridCanvas;
        private HexMesh _mesh;

        private void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();
            _mesh = GetComponentInChildren<HexMesh>();

            _cells = new HexCell[Width * Height];
            for (int z = 0, i = 0; z < Height; z++)
            {
                for (var x = 0; x < Width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void Start()
        {
            _mesh.Triangulate(_cells);
        }

        public void ColorCell(Vector3 position, Color color)
        {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);

            var index = coordinates.X + coordinates.Z * Width + coordinates.Z / 2;
            var cell = _cells[index];
            cell.Color = color;
            _mesh.Triangulate(_cells);
        }

        private void CreateCell(int x, int z, int i)
        {
            // get world position
            var position = new Vector3
            {
                // ReSharper disable once PossibleLossOfFraction
                // reason: (integer division)
                x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f),
                y = 0f,
                z = z * (HexMetrics.OuterRadius * 1.5f)
            };

            // create cell
            var cell = _cells[i] = Instantiate(CellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = DefaultColor;

            SetNeighbors(x, z, i, cell);

            // create label
            var label = Instantiate(CellLabelPrefab);
            label.rectTransform.SetParent(_gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.Coordinates.ToStringOnSeparateLines();
        }

        private void SetNeighbors(int x, int z, int i, HexCell cell)
        {
            if (x > 0)
            {
                // W <-> E Neighbors
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            }
            if (z <= 0)
            {
                // no z neighbors have been created yet, bail
                return;
            }
            
            // are we in an even z row
            if ((z & 1) == 0)
            {
                // SE <-> NW
                cell.SetNeighbor(HexDirection.SE, _cells[i - Width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - Width - 1]);
                }
            }
            else
            {
                // SW <-> NE
                cell.SetNeighbor(HexDirection.SW, _cells[i - Width]);
                if (x < Width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - Width + 1]);
                }
            }
        }
    }
}