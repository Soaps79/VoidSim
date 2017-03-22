using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
    //todo: Upgrade QGame unity reference, currently cant reference UnityEngine.EventSystems
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
