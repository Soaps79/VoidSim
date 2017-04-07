﻿using Assets.Utility.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.HexGrid.Scripts
{
    public class HexMapEditor : MonoBehaviour
    {
        private enum OptionalToggle { Ignore = 0, Yes, No };
        
        [RequireReference]
        [SerializeField]
        public HexGrid HexGrid;

        //private Color _activeColor;
        private int _activeElevation;
        private int _activeWaterLevel;
        private int _activeSpecialIndex;
        private int _brushSize = 0;

        private int _activeUrbanLevel;
        private int _activeFarmLevel;
        private int _activePlantLevel;

        private int _activeTerrainTypeIndex;

        //private bool _applyColor;
        private bool _applyElevation;
        private bool _applyWaterLevel;

        private bool _applyUrbanLevel;
        private bool _applyFarmLevel;
        private bool _applyPlantLevel;

        private bool _applySpecialIndex;

        private OptionalToggle _riverMode = 0;
        private OptionalToggle _walledMode = 0;
        
        private bool _isDrag;

        private HexDirection _dragDirection;
        private HexCell _prevCell;

        private void Awake()
        {
            //SetActiveColor(0);
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

            EditBasicBrush(cell);
            EditFeatures(cell);
            EditRiver(cell);
        }

        private void EditRiver(HexCell cell)
        {
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

        private void EditFeatures(HexCell cell)
        {
            if (_applySpecialIndex)
            {
                cell.SpecialIndex = _activeSpecialIndex;
            }

            if (_applyUrbanLevel)
            {
                cell.UrbanLevel = _activeUrbanLevel;
            }

            if (_applyFarmLevel)
            {
                cell.FarmLevel = _activeFarmLevel;
            }

            if (_applyPlantLevel)
            {
                cell.PlantLevel = _activePlantLevel;
            }

            if (_walledMode != OptionalToggle.Ignore)
            {
                cell.IsWalled = _walledMode == OptionalToggle.Yes;
            }
        }

        private void EditBasicBrush(HexCell cell)
        {
            if (_activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = _activeTerrainTypeIndex;
            }

            if (_applyElevation)
            {
                cell.Elevation = _activeElevation;
            }

            if (_applyWaterLevel)
            {
                cell.WaterLevel = _activeWaterLevel;
            }
        }

        #region Editor UI Controls

        // --- Set should apply ---
        public void SetApplyWaterLevel(bool toggle)
        {
            _applyWaterLevel = toggle;
        }

        public void SetApplyElevation(bool toggle)
        {
            _applyElevation = toggle;
        }

        public void SetApplyUrbanLevel(bool toggle)
        {
            _applyUrbanLevel = toggle;
        }

        public void SetApplyFarmLevel(bool toggle)
        {
            _applyFarmLevel = toggle;
        }

        public void SetApplyPlantLevel(bool toggle)
        {
            _applyPlantLevel = toggle;
        }

        public void SetApplySpecialIndex(bool toggle)
        {
            _applySpecialIndex = toggle;
        }

        // --- Set values ---
        public void SetRiverMode(int mode)
        {
            _riverMode = (OptionalToggle)mode;
        }

        public void SetWalledMode(int mode)
        {
            _walledMode = (OptionalToggle)mode;
        }

        public void SetWaterLevel(float level)
        {
            _activeWaterLevel = (int)level;
        }

        public void SetTerrainTypeIndex(int index)
        {
            _activeTerrainTypeIndex = index;
        }

        public void SetElevation(float elevation)
        {
            _activeElevation = (int)elevation;
        }

        public void SetBrushSize(float size)
        {
            _brushSize = (int)size;
        }

        public void SetUrbanLevel(float level)
        {
            _activeUrbanLevel = (int)level;
        }

        public void SetFarmLevel(float level)
        {
            _activeFarmLevel = (int)level;
        }

        public void SetPlantLevel(float level)
        {
            _activePlantLevel = (int)level;
        }

        public void SetSpecialIndex(int index)
        {
            _activeSpecialIndex = index;
        }

        // misc.
        public void ShowUI(bool isVisible)
        {
            HexGrid.ShowUI(isVisible);
        }

        #endregion Editor UI Controls
    }
}