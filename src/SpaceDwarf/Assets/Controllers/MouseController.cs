using System;
using Assets.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Controllers
{
    public class MouseController : SingletonBehavior<MouseController>
    {
        //todo: DI layer masks
        private const int PlayerLayerMask = 1 << 11;
        private const int UnitsLayerMask = 1 << 10;
        private const int BuildingsLayerMask = 1 << 9;
        private const int TerrainLayerMask = 1 << 8;

        private GameObject _underMouse;
        public GameObject UnderMouse
        {
            get { return _underMouse; }
            private set
            {
                if (HasChanged(_underMouse, value))
                {
                    var prev = _underMouse;
                    _underMouse = value;
                    if (OnUnderMouseChanged != null)
                        OnUnderMouseChanged(prev, value);
                }
            }
        }

        public Vector3 RawPosition { get; private set; }

        public Action<GameObject, GameObject> OnUnderMouseChanged;

        protected override void OnUpdateStart(float delta)
        {
            base.OnUpdateStart(delta);
            RawPosition = Input.mousePosition;
            UnderMouse = GetObjectUnderMouse(CameraController.Instance.ActiveCamera);
        }

        private static bool HasChanged(Object prev, Object next)
        {
            return (prev != null && next == null)
                   || (prev == null && next != null)
                   || (prev != null && prev.name != next.name);
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
