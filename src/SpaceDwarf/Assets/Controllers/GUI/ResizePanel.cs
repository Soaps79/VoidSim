using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
    //todo: Upgrade QGame unity reference, currently cant reference UnityEngine.EventSystems
    public class ResizePanel : OrderedEventBehavior, IPointerDownHandler, IDragHandler
    {
        public Vector2 MinSize = new Vector2(30, 40);
        public Vector2 MaxSize = new Vector2(1920, 1080);

        private RectTransform _transform;
        private Vector2 _currentPointer;
        private Vector2 _previousPointer;

        protected override void OnAwake()
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

            // only resize if we are at the extreme bottom or right, minus a modest buffer zone
            const int buffer = 20;
            var nearBottom = _currentPointer.y < _transform.rect.position.y + buffer;
            if (!nearBottom)
            {
                // not near the bottom, don't allow negative y resize
                resizeValue.y = Mathf.Max(resizeValue.y, 0);
            }
            var nearRight = _currentPointer.x > _transform.rect.position.x + _transform.rect.width - buffer;
            if (!nearRight)
            {
                // not near the right, don't allow positive x resize
                resizeValue.x = Mathf.Min(resizeValue.x, 0);
            }

            //Debug.Log(string.Format("NearBottom:{0}; NearRight:{1}", nearBottom, nearRight));

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