using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public class FixedCamera : CameraBase
    {
        public Vector3 LookAt;
        public float Offset = -10;

        public FixedCamera()
        {
            IsPhysicsBased = false;
        }
        
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            transform.position = new Vector3(LookAt.x, LookAt.y, LookAt.z + Offset);
        }
    }
}
