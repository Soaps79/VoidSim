using UnityEngine;

namespace Assets.Controllers.Cameras
{
    [CreateAssetMenu(menuName = "Cameras/Controls/Mouse Zoom")]
    public class MouseZoomControl : CameraControl 
    {
        public override string Name { get { return "MouseZoomControl"; } }

        public int MaxOrthoSize = 100;
        public int MinOrthoSize = 8;
        public int ZoomSpeed = 100;

        public override void Execute(ICamera camera, float timeDelta)
        {
            HandleMouseZoom(camera, timeDelta);
        }

        private void HandleMouseZoom(ICamera camera, float timeDelta)
        {            
            var magnitude = timeDelta * ZoomSpeed;

            // backward
            if (Input.GetAxis("Zoom") < 0)
            {
                camera.CameraComponent.orthographicSize = Mathf.Min(
                    camera.CameraComponent.orthographicSize + magnitude, 
                    MaxOrthoSize);
            }

            // forward
            if (Input.GetAxis("Zoom") > 0)
            {
                camera.CameraComponent.orthographicSize = Mathf.Max(
                    camera.CameraComponent.orthographicSize - magnitude, 
                    MinOrthoSize);
            }
        }
    }
}