using System.Collections.Generic;
using QGame;
using UnityEngine;

namespace Assets.Controllers
{
    // todo: this is using drop-in instances to cameras.
    // figure out a good way to:
    //   1.) Dynamically add cameras created at runtime
    //   2.) Dynamically add cameras in editor
    public class CameraController : SingletonBehavior<CameraController>
    {
        // Keys for camera scripting
        // todo: use SOs, as-in Types example
        public const string FreeCameraKey = "FreeCamera";
        public const string FollowCameraKey = "FollowCamera";
        public const string SpringCameraKey = "SpringCamera";

        public Camera FreeCamera;
        public Camera FollowCamera;
        public Camera SpringCamera;

        private Camera _activeCamera;
        private Camera _prevCamera;

        /// <summary>
        /// Currently active camera component.
        /// </summary>
        public Camera ActiveCamera { get { return _activeCamera; } }
        
        
        /// <summary>
        /// Camera map to resolve string keys to in scene Camera elements.
        /// </summary>
        private readonly Dictionary<string, Camera> _cameraMap = new Dictionary<string, Camera>();

        protected override void OnStart()
        {
            base.OnStart();

            // start with FollowCamera by default
            FollowCamera.enabled = true;
            _prevCamera = _activeCamera = FollowCamera;
            
            // disable other camera types
            FreeCamera.enabled = false;
            SpringCamera.enabled = false;
            
            // build map of known set camera types
            _cameraMap.Add(FreeCameraKey, FreeCamera);
            _cameraMap.Add(FollowCameraKey, FollowCamera);
            _cameraMap.Add(SpringCameraKey, SpringCamera);

            // override active camera
            ChangeCamera(SpringCameraKey);
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

        public void ChangeCamera(Camera cameraComponent)
        {
            // disable all
            foreach (var sceneCamera in _cameraMap.Values)
            {
                sceneCamera.enabled = false;
            }

            // activate camera passed in, save previous
            _prevCamera = _activeCamera;
            _activeCamera = cameraComponent;
            cameraComponent.enabled = true;

            // handle special case:
            // Free Cam should center on player when enabled.
            // todo: consider moving this somewhere?
            //if(cameraComponent == FreeCamera)
            //    CenterCameraOnPlayer(FreeCamera);
        }

        //private static void CenterCameraOnPlayer(Component cameraComponent)
        //{
        //    var playerPosition = PlayerController.Instance.PlayerCharacter.Position;
        //    cameraComponent.transform.position = new Vector3(
        //        playerPosition.x, 
        //        playerPosition.y, 
        //        cameraComponent.transform.position.z);
        //}

        public void Revert()
        {
            ChangeCamera(_prevCamera);
        }
    }
}
