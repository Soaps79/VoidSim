using Assets.Utility.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.HexGrid.Scripts
{
    public class HexGrid : MonoBehaviour
    {
        public int ChunkCountX = 4;
        public int ChunkCountZ = 3;

        private int _cellCountX;
        private int _cellCountZ;

        public Color DefaultColor = Color.white;

        [RequireReference]
        public HexCell CellPrefab;

        [RequireReference]
        public Text CellLabelPrefab;

        [RequireReference]
        public Texture2D NoiseSource;

        [RequireReference]
        public HexGridChunk ChunkPrefab;

        private HexCell[] _cells;
        //private Canvas _gridCanvas;
        //private HexMesh _mesh;

        private HexGridChunk[] _chunks;

        private void Awake()
        {
            HexMetrics.NoiseSource = NoiseSource;

            //_gridCanvas = GetComponentInChildren<Canvas>();
            //_mesh = GetComponentInChildren<HexMesh>();

            _cellCountX = ChunkCountX * HexMetrics.ChunkSizeX;
            _cellCountZ = ChunkCountZ * HexMetrics.ChunkSizeZ;

            CreateChunks();
            CreateCells();
        }

        private void CreateChunks()
        {
            _chunks = new HexGridChunk[ChunkCountX * ChunkCountZ];

            for (int z = 0, i = 0; z < ChunkCountZ; z++)
            {
                for (var x = 0; x < ChunkCountX; x++)
                {
                    var chunk = _chunks[i++] = Instantiate(ChunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }

        private void CreateCells()
        {
            _cells = new HexCell[_cellCountX * _cellCountZ];
            for (int z = 0, i = 0; z < _cellCountZ; z++)
            {
                for (var x = 0; x < _cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        void OnEnable()
        {
            HexMetrics.NoiseSource = NoiseSource;
        }

        //private void Start()
        //{
        //    _mesh.Triangulate(_cells);
        //}

        //public void Refresh()
        //{
        //    _mesh.Triangulate(_cells);
        //}

        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            var index = coordinates.X + coordinates.Z * _cellCountX + coordinates.Z / 2;
            var cell = _cells[index];
            return cell;
        }

        public HexCell GetCell(HexCoordinates coords)
        {
            var z = coords.Z;
            if(z < 0 || z >= _cellCountZ) { return null; }

            var x = coords.X + z / 2;
            if(x < 0 || x >= _cellCountX) { return null; }

            return _cells[x + z * _cellCountX];
        }

        public void ShowUI(bool isVisible)
        {
            for (var i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].ShowUI(isVisible);
            }
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
            //cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = DefaultColor;

            SetNeighbors(x, z, i, cell);

            // create label
            var label = Instantiate(CellLabelPrefab);
            //label.rectTransform.SetParent(_gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.Coordinates.ToStringOnSeparateLines();
            cell.UiRect = label.rectTransform;

            // set elevation to trigger perturb logic
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            var chunkX = x / HexMetrics.ChunkSizeX;
            var chunkZ = z / HexMetrics.ChunkSizeZ;
            var chunk = _chunks[chunkX + chunkZ * ChunkCountX];

            var localX = x - chunkX * HexMetrics.ChunkSizeX;
            var localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);


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
                cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX - 1]);
                }
            }
            else
            {
                // SW <-> NE
                cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX]);
                if (x < _cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX + 1]);
                }
            }
        }
    }
}