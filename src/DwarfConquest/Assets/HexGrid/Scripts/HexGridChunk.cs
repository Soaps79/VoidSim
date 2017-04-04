using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexGridChunk : MonoBehaviour
    {
        private HexCell[] _cells;
        private HexMesh _mesh;
        private Canvas _gridCanvas;

        void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();
            _mesh = GetComponentInChildren<HexMesh>();

            _cells = new HexCell[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];
        }

        void Start()
        {
            //_mesh.Triangulate(_cells);
        }

        public void AddCell(int index, HexCell cell)
        {
            _cells[index] = cell;
            cell.Chunk = this;
            cell.transform.SetParent(transform, false);
            cell.UiRect.SetParent(_gridCanvas.transform, false);
        }

        public void Refresh()
        {
            // activate this component so that triangulation is called once 
            // after the frame has been completely updated
            enabled = true;
        }

        public void ShowUI(bool isVisible)
        {
            _gridCanvas.gameObject.SetActive(isVisible);
        }

        void LateUpdate()
        {
            _mesh.Triangulate(_cells);
            enabled = false;
        }
    }
}
