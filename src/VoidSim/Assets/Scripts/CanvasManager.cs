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
        private Canvas _screenspace_root;
        private Transform _worldspace_root;

        void Awake()
        {
            CreateScreenSpaceCanvases();
            CreateWorldSpaceCanvases();
        }

        private void CreateWorldSpaceCanvases()
        {
            _worldspace_root = new GameObject("worldspace_root").transform;
            //_worldspace_root.worldCamera = _mainCamera;
            //_worldspace_root.name = "worldspace_root";

            var canvas = Instantiate(_worldSpacePrefab, _worldspace_root, false);
            canvas.name = "occupancy";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.Occupancy, canvas);
        }

        private void CreateScreenSpaceCanvases()
        {
            _screenspace_root = Instantiate(_screenSpacePrefab);
            _screenspace_root.worldCamera = _mainCamera;
            _screenspace_root.name = "screenspace_root";

            var canvas = Instantiate(_screenSpacePrefab, _screenspace_root.transform, false);
            canvas.name = "constant_update";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.ConstantUpdate, canvas);

            canvas = Instantiate(_screenSpacePrefab, _screenspace_root.transform, false);
            canvas.name = "medium_update";
            canvas.worldCamera = _mainCamera;
            _canvases.Add(CanvasType.MediumUpdate, canvas);

            canvas = Instantiate(_screenSpacePrefab, _screenspace_root.transform, false);
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