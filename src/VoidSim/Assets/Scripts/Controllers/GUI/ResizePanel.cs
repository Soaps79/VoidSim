using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
    public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        public Vector2 MinSize = new Vector2(30, 40);
        public Vector2 MaxSize = new Vector2(1920, 1080);

        private RectTransform _transform;
        private Vector2 _currentPointer;
        private Vector2 _previousPointer;

        void Awake()
        {
            _transform = transform.parent.GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData data)
        {
            _transform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, data.position, data.pressEventCamera, out _previousPointer);
        }

        public void OnDrag(PointerEventData data)
        {
            if (_transform == null)
                return;

            var sizeDelta = _transform.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, data.position, data.pressEventCamera, out _currentPointer);
            var resizeValue = _currentPointer - _previousPointer;

            sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);
            sizeDelta = new Vector2(
                Mathf.Clamp(sizeDelta.x, MinSize.x, MaxSize.x),
                Mathf.Clamp(sizeDelta.y, MinSize.y, MaxSize.y)
            );

            _transform.sizeDelta = sizeDelta;

            _previousPointer = _currentPointer;
        }
    }
}