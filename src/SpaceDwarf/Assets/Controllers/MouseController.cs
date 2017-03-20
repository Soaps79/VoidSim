using System;
using Assets.Framework;
using UnityEngine;

namespace Assets.Controllers
{
    public class MouseController : SingletonBehavior<MouseController>
    {
        // todo: centralize
        private const int PlayerLayerMask = 1 << 11;
        private const int UnitsLayerMask = 1 << 10;
        private const int BuildingsLayerMask = 1 << 9;
        private const int TerrainLayerMask = 1 << 8;
        public GameObject UnderMouse { get; private set; }

        private GameObject _previousUnderMouse = null;

        public Vector3 RawPosition { get; private set; }

        public Action<GameObject, GameObject> OnUnderMouseChanged;

        protected override void OnUpdateStart(float delta)
        {
            base.OnUpdateStart(delta);
            RawPosition = Input.mousePosition;
            UnderMouse = GetObjectUnderMouse(CameraController.Instance.ActiveCamera);

            if (HasUnderMouseChanged())
            {
                // switched, fire event
                if (OnUnderMouseChanged != null)
                    OnUnderMouseChanged(_previousUnderMouse, UnderMouse);
            }

            // save for next frame
            _previousUnderMouse = UnderMouse;
        }

        private bool HasUnderMouseChanged()
        {
            return (UnderMouse == null && _previousUnderMouse != null)
                   || (_previousUnderMouse == null && UnderMouse != null)
                   || (_previousUnderMouse != null && UnderMouse != null
                       && _previousUnderMouse.name != UnderMouse.name);
        }

        private GameObject GetObjectUnderMouse(Camera activeCamera)
        {
            // project ray from screen into world
            var ray = activeCamera.ScreenPointToRay(Input.mousePosition);

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
    }
}
