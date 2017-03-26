using QGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Controllers.GUI
{
    public class TooltipController : SingletonBehavior<TooltipController>
    {
        private string _tooltipText1;
        private string _tooltipText2;

        public Text TopText;
        public Text BottomText;

        public Text Label;
        public Text FlavorText;
        public Image Thumbnail;
        public Text Coords;

        public void SetTooltip(string label, string flavor, Sprite thumbnail, Vector2 coords)
        {
            Label.text = label;
            FlavorText.text = flavor;
            Thumbnail.sprite = thumbnail;
            Coords.text = string.Format("({0:00}, {1:00})", coords.x, coords.y);
        }
        
        protected override void OnGUIDraw(float delta)
        {
            base.OnGUIDraw(delta);

            //// get game object under mouse
            //var underMouse = MouseController.Instance.UnderMouse;
            //SetTooltip(underMouse);
        }

        private void SetTooltip(GameObject go)
        {
            if (go == null)
                return;

            var tooltip = go.GetComponent<TooltipBehavior>();
            if (tooltip == null)
                return;

            if (TopText == null || BottomText == null)
                return;

            if (!string.Equals(_tooltipText1, tooltip.TooltipText1))
            {
                _tooltipText1 = tooltip.TooltipText1;
                TopText.text = _tooltipText1;
            }

            if (!string.Equals(_tooltipText2, tooltip.TooltipText2))
            {
                _tooltipText2 = tooltip.TooltipText2;
                BottomText.text = _tooltipText2;
            }
        }
    }
}
