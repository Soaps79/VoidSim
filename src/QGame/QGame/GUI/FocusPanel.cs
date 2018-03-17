using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QGame.GUI
{
    public class FocusPanel : OrderedEventBehavior, IPointerDownHandler
    {
        private RectTransform _panel;

        void Awake()
        {
            _panel = GetComponent<RectTransform>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _panel.SetAsLastSibling();
        }
    }
}
