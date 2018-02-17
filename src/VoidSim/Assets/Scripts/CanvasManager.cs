using System.Collections.Generic;
using QGame;
using UnityEngine;

namespace Assets.Scripts
{
    public enum CanvasType
    {
        ConstantUpdate, MediumUpdate, LowUpdate
    }

    public interface ICanvasManager
    {
        Canvas GetCanvas(CanvasType type);
    }

    public class CanvasManager : QScript, ICanvasManager
    {
        [SerializeField] private Canvas _prefabCanvas;
        private readonly Dictionary<CanvasType, Canvas> _canvases = new Dictionary<CanvasType, Canvas>();
        private Canvas _mainCanvas;

        void Start()
        {
            _mainCanvas = Instantiate(_prefabCanvas);
            _mainCanvas.name = "canvas_root";

            var canvas = Instantiate(_prefabCanvas, _mainCanvas.transform, false);
            canvas.name = "constant_update";
            _canvases.Add(CanvasType.ConstantUpdate, canvas);

            canvas = Instantiate(_prefabCanvas, _mainCanvas.transform, false);
            canvas.name = "medium_update";
            _canvases.Add(CanvasType.MediumUpdate, canvas);

            canvas = Instantiate(_prefabCanvas, _mainCanvas.transform, false);
            canvas.name = "low_update";
            _canvases.Add(CanvasType.LowUpdate, canvas);
        }

        public Canvas GetCanvas(CanvasType type)
        {
            return _canvases[type];
        }
    }
}