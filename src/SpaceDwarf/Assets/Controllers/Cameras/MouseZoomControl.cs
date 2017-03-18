using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public class MouseZoomControl<T> : CameraControl<T>  where T : ICamera 
    {
        public override string Name { get { return "MouseZoomControl"; } }

        public override void Execute(T camera, float timeDelta)
        {
            HandleMouseZoom(camera, timeDelta);
        }

        private static void HandleMouseZoom(T camera, float timeDelta)
        {            
            var magnitude = timeDelta * camera.CameraSettings.ZoomSpeed;

            // backward
            if (Input.GetAxis("Zoom") < 0)
            {
                Camera.main.orthographicSize = Mathf.Min(
                    Camera.main.orthographicSize + magnitude, 
                    camera.CameraSettings.MaxOrthoSize);
            }

            // forward
            if (Input.GetAxis("Zoom") > 0)
            {
                Camera.main.orthographicSize = Mathf.Max(
                    Camera.main.orthographicSize - magnitude, 
                    camera.CameraSettings.MinOrthoSize);
            }
        }
    }
}