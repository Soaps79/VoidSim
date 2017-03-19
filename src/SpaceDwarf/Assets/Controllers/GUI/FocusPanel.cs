using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
    public class FocusPanel : OrderedEventBehavior, IPointerDownHandler
    {
        private RectTransform _panel;

        protected override void OnAwake()
        {
            _panel = GetComponent<RectTransform>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _panel.SetAsLastSibling();
        }
    }
}
