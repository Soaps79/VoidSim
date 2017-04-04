using System;
using Assets.Utility.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.HexGrid.Scripts
{
    public class HexMapEditor : MonoBehaviour
    {
        private enum OptionalToggle { Ignore=0, Yes, No };

        public Color[] Colors;

        [RequireReference]
        [SerializeField]
        public HexGrid HexGrid;

        private Color _activeColor;
        private int _activeElevation;
        private int _brushSize = 0;

        private bool _applyColor;
        private bool _applyElevation = true;

        private OptionalToggle _riverMode = 0;

        // drag support
        private bool _isDrag;
        private HexDirection _dragDirection;
        private HexCell _prevCell;

        private void Awake()
        {
            SelectColor(0);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
            else
            {
                _prevCell = null;
            }
        }

        private void HandleInput()
        {
            var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                var currentCell = HexGrid.GetCell(hit.point);

                if (_prevCell != null && _prevCell != currentCell)
                {
                    ValidateDrag(currentCell);
                }
                else
                {
                    _isDrag = false;
                }

                EditCells(currentCell);
                _prevCell = currentCell;
            }
            else
            {
                _prevCell = null;
            }
        }

        private void ValidateDrag(HexCell currentCell)
        {
            for (_dragDirection = HexDirection.NE; _dragDirection <= HexDirection.NW; _dragDirection++)
            {
                if (_prevCell.GetNeighbor(_dragDirection) == currentCell)
                {
                    _isDrag = true;
                    return;
                }
            }
            _isDrag = false;
        }

        private void EditCells(HexCell center)
        {
            var centerX = center.Coordinates.X;
            var centerZ = center.Coordinates.Z;

            // grow from bottom to center
            for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
            {
                for (var x = centerX - r; x <= centerX + _brushSize; x++)
                {
                    EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            // grow from top to center, omitting center row
            for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
            {
                for (var x = centerX - _brushSize; x <= centerX + r; x++)
                {
                    EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }

        }

        private void EditCell(HexCell cell)
        {
            if (cell == null) { return; }

            if (_applyColor)
            {
                cell.Color = _activeColor;
            }
            if (_applyElevation)
            {
                cell.Elevation = _activeElevation;
            }

            if (_riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            else if (_isDrag && _riverMode == OptionalToggle.Yes)
            {
                var otherCell = cell.GetNeighbor(_dragDirection.Opposite());
                if (otherCell != null)
                {
                    otherCell.SetOutgoingRiver(_dragDirection);
                }
            }
        }

        public void SelectColor(int i)
        {
            _applyColor = i >= 0;
            if (_applyColor)
            {
                _activeColor = Colors[i];
            }
        }

        public void SetElevation(float elevation)
        {
            _activeElevation = (int)elevation;
        }

        public void SetApplyElevation(bool toggle)
        {
            _applyElevation = toggle;
        }

        public void SetBrushSize(float size)
        {
            _brushSize = (int) size;
        }

        public void ShowUI(bool isVisible)
        {
            HexGrid.ShowUI(isVisible);
        }

        public void SetRiverMode(int mode)
        {
            _riverMode = (OptionalToggle) mode;
        }
    }
}