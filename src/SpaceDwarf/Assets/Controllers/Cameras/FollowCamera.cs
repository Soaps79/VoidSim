using Assets.Model;
using UnityEngine;
using Zenject;

namespace Assets.Controllers.Cameras
{
    public class FollowCamera : CameraBase
    {
        public GameObject LookAt;
        public float Offset = -10;

        public FollowCamera()
        {
            // todo: determine if LookAt is physically simulated or not
            IsPhysicsBased = false;
        }
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            transform.position = new Vector3(
                LookAt.transform.position.x,
                LookAt.transform.position.y,
                LookAt.transform.position.z + Offset);
        }
    }
}
