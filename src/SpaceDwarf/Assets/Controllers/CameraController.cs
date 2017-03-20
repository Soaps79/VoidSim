using Assets.Controllers.Cameras;
using Assets.Framework;
using UnityEngine;

namespace Assets.Controllers
{
    public class CameraController : SingletonBehavior<CameraController>
    {
        public GameObject MainCamera;
        public Camera ActiveCamera { get { return MainCamera.GetComponent<Camera>(); } }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public void ChangeCamera(GameObject cameraObject)
        {
        }
        public void Revert()
        {
        }

        private void SwapCamera(GameObject nextCameraObject)
        {
            
        }
    }
}
