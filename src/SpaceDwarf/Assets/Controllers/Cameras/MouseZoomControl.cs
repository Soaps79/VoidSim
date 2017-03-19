using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public class MouseZoomControl : CameraControl 
    {
        public override string Name { get { return "MouseZoomControl"; } }

        public override void Execute(ICamera camera, float timeDelta)
        {
            HandleMouseZoom(camera, timeDelta);
        }

        private static void HandleMouseZoom(ICamera camera, float timeDelta)
        {            
            var magnitude = timeDelta * camera.CameraSettings.ZoomSpeed;

            // backward
            if (Input.GetAxis("Zoom") < 0)
            {
                camera.CameraComponent.orthographicSize = Mathf.Min(
                    camera.CameraComponent.orthographicSize + magnitude, 
                    camera.CameraSettings.MaxOrthoSize);
            }

            // forward
            if (Input.GetAxis("Zoom") > 0)
            {
                camera.CameraComponent.orthographicSize = Mathf.Max(
                    camera.CameraComponent.orthographicSize - magnitude, 
                    camera.CameraSettings.MinOrthoSize);
            }
        }
    }
}