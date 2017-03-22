using System.Collections.Generic;
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

        private readonly Dictionary<string, Camera> _cameraMap = new Dictionary<string, Camera>();

        protected override void OnStart()
        {
            base.OnStart();
            FreeCamera.enabled = false;
            FollowCamera.enabled = true;
            _prevCamera = _activeCamera =  FollowCamera;
            _cameraMap.Add("FreeCamera", FreeCamera);
            _cameraMap.Add("FollowCamera", FollowCamera);
        }

        public void ChangeCamera(string cameraKey)
        {
            Camera cam;
            if (_cameraMap.TryGetValue(cameraKey, out cam))
            {
                ChangeCamera(cam);
            }
            else
            {
                Debug.LogWarning(string.Format("Unknown camera key: \'{0}\'.", cameraKey));
            }
        }

        public void ChangeCamera(Camera cameraObject)
        {
            if (cameraObject == FreeCamera)
            {
                CenterCameraOnPlayer(FreeCamera);
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

        private void CenterCameraOnPlayer(Camera cameraObject)
        {
            var playerPosition = PlayerController.Instance.PlayerCharacter.Position;
            cameraObject.transform.position = new Vector3(
                playerPosition.x, 
                playerPosition.y, 
                cameraObject.transform.position.z);
        }

        public void Revert()
        {
            ChangeCamera(_prevCamera);
        }
    }
}
