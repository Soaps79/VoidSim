using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.GUI
{
    public class FocusPanel : QScript, IPointerDownHandler
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
