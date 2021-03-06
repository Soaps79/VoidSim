﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace QGame.GUI
{
    public class DragPanel : OrderedEventBehavior, IPointerDownHandler, IDragHandler
    {
        private Vector2 _pointerOffset;
        private RectTransform _canvasTransform;
        private RectTransform _panelTransform;

        public GameObject Panel;

        void Start()
        {
            if (Panel == null)
            {
                throw new ArgumentException("Must set the panel to drag.");
            }

            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                throw new ArgumentException("Cannot add a panel without a canvas");

            }
            _canvasTransform = canvas.transform as RectTransform;
            _panelTransform = Panel.transform as RectTransform;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _panelTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _panelTransform,
                eventData.position,
                eventData.pressEventCamera,
                out _pointerOffset);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_panelTransform == null)
                return;

            // clamp pointer so panel doesn't drag off screen
            Vector2 pointerPosition = ClampToWindow(eventData.position);

            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasTransform,
                pointerPosition,
                eventData.pressEventCamera,
                out localPointerPosition))
            {
                _panelTransform.localPosition = localPointerPosition - _pointerOffset;
            }
        }

        private Vector2 ClampToWindow(Vector2 position)
        {
            // get corners of canvas
            var canvasCorners = new Vector3[4];
            _canvasTransform.GetWorldCorners(canvasCorners);

            var clampedX = Mathf.Clamp(position.x, canvasCorners[0].x, canvasCorners[2].x);
            var clampedY = Mathf.Clamp(position.y, canvasCorners[0].y, canvasCorners[2].y);

            return new Vector2(clampedX, clampedY);
        }
    }
}
