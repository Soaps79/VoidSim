using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public class PanAndScanControl : CameraControl
    {
        public override string Name { get { return "PanAndScanControl"; } }
        public override void Execute(ICamera camera, float timeDelta)
        {
            var magnitude = timeDelta * camera.CameraSettings.MoveSpeed;
            var horizontal = Input.GetAxis("Horizontal") * magnitude;
            var vertical = Input.GetAxis("Vertical") * magnitude;
            var movement = new Vector3(horizontal, vertical, 0);

            camera.CameraComponent.transform.position = new Vector3(
                camera.CameraComponent.transform.position.x + movement.x,
                camera.CameraComponent.transform.position.y + movement.y,
                camera.CameraComponent.transform.position.z + movement.z);
        }
    }
}
