using System.Collections.Generic;
using QGame;
using UnityEngine;

namespace Assets.Scripts
{
    public enum CanvasType
    {
        ConstantUpdate, MediumUpdate, LowUpdate, Occupancy
    }

    public interface ICanvasManager
    {
        Canvas GetCanvas(CanvasType type);
    }

    public class CanvasManager : QScript, ICanvasManager
    {
        [SerializeField] private Canvas _screenSpacePrefab;
        [SerializeField] private Canvas _worldSpacePrefab;
        [SerializeField] private Camera _mainCamera;
        private readonly Dictionary<CanvasType, Canvas> _canvases = new Dictionary<CanvasType, Canvas>();
        
        // figure out using Canvas here vs using Transform
        private Canvas _screenspaceRoot;
        private Transform _worldspaceRoot;
        private Transform _uiRoot;

        void Awake()
        {
            _uiRoot = new GameObject("ui_root").transform;
            CreateScreenSpaceCanvases();
            CreateWorldSpaceCanvases();
        }

        private void CreateWorldSpaceCanvases()
        {
            _worldspaceRoot = new GameObject("worldspace_root").transform;
            _worldspaceRoot.SetParent(_uiRoot);

            var canvas = Instantiate(_worldSpacePrefab, _worldspaceRoot, false);
            canvas.name = "occupancy";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.Occupancy, canvas);
        }

        private void CreateScreenSpaceCanvases()
        {
            _screenspaceRoot = Instantiate(_screenSpacePrefab, _uiRoot);
            _screenspaceRoot.worldCamera = _mainCamera;
            _screenspaceRoot.name = "screenspace_root";

            var canvas = Instantiate(_screenSpacePrefab, _screenspaceRoot.transform, false);
            canvas.name = "constant_update";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.ConstantUpdate, canvas);

            canvas = Instantiate(_screenSpacePrefab, _screenspaceRoot.transform, false);
            canvas.name = "medium_update";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.MediumUpdate, canvas);

            canvas = Instantiate(_screenSpacePrefab, _screenspaceRoot.transform, false);
            canvas.name = "low_update";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.LowUpdate, canvas);
        }

        public Canvas GetCanvas(CanvasType type)
        {
            return _canvases[type];
        }
    }
}