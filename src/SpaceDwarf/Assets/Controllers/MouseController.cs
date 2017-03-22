using System;
using QGame;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Controllers
{
    public class MouseController : SingletonBehavior<MouseController>
    {
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
            var worldMouse = activeCamera.ScreenToWorldPoint(RawPosition);
            
            var hit = Physics2D.Raycast(worldMouse, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject != null)
            {
                return hit.collider.gameObject;
            }

            return null;

        }
    }
}
