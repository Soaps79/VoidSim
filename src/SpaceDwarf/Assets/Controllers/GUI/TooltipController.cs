using QGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Controllers.GUI
{
    public class TooltipController : OrderedEventBehavior
    {
        private string _tooltipText1;
        private string _tooltipText2;

        public Text TopText;
        public Text BottomText;

        //todo: centralize
        private const int PlayerLayerMask = 1 << 11;
        private const int UnitsLayerMask = 1 << 10;
        private const int BuildingsLayerMask = 1 << 9;
        private const int TerrainLayerMask = 1 << 8;

        [Inject] public CameraController CameraController;
        
        protected override void OnGUIDraw(float delta)
        {
            base.OnGUIDraw(delta);

            // get game object under mouse
            var underMouse = GetObjectUnderMouse();
            SetTooltip(underMouse);
        }

        private GameObject GetObjectUnderMouse()
        {
            // project ray from screen into world
            var camera = CameraController.ActiveCamera;
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            GameObject foundGo = null;
            
            // use layers to avoid z-fighting
            // Player -> Units -> Buildings -> Terrain
            if (Physics.Raycast(ray, out hit, 100, PlayerLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, UnitsLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, BuildingsLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, TerrainLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }

            return foundGo;
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
