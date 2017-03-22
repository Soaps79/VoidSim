using Assets.Controllers.Cameras;
using Assets.Framework;
using UnityEngine;

namespace Assets.Controllers
{
    public class CameraController : SingletonBehavior<CameraController>
    {
        private Camera _activeCamera;
        private Camera _prevCamera;
        public Camera ActiveCamera { get { return _activeCamera; } }

        public Camera FreeCamera;
        public Camera FollowCamera;

        protected override void OnStart()
        {
            base.OnStart();
            FreeCamera.enabled = false;
            FollowCamera.enabled = true;
            _prevCamera = _activeCamera =  FollowCamera; 
        }

        public void ChangeCamera(Camera cameraObject)
        {
            if (cameraObject == FreeCamera)
            {
                FreeCamera.enabled = true;
                FollowCamera.enabled = false;
                _prevCamera = _activeCamera;
                _activeCamera = FreeCamera;
            }
            else if (cameraObject == FollowCamera)
            {
                FreeCamera.enabled = false;
                FollowCamera.enabled = true;
                _prevCamera = _activeCamera;
                _activeCamera = FollowCamera;
            }
            else
            {
                Debug.LogWarning("Switching to unrecognized camera. Disabling Camera Controller registered cameras.");
                FreeCamera.enabled = false;
                FollowCamera.enabled = false;
                cameraObject.enabled = true;
                _prevCamera = _activeCamera;
                _activeCamera = cameraObject;
            }
        }
        public void Revert()
        {
            ChangeCamera(_prevCamera);
        }
    }
}
