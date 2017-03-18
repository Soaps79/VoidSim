using System.Collections.Generic;
using Assets.Configuration;
using Assets.Framework;
using UnityEngine;
using Zenject;

namespace Assets.Controllers.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraBase : MonoBehaviour, ICamera
    {
        //todo: not this
        [Inject] public CameraSettings CameraSettings { get; set; }
        public Vector3 Position { get { return transform.position; } }
        public Vector3 Forward { get { return transform.forward; } }
        public Vector3 Up { get { return transform.up; } }
        public Vector3 Right { get { return transform.right; } }

        private readonly Dictionary<string, CameraControl<CameraBase>> _controls
            = new Dictionary<string, CameraControl<CameraBase>>();

        protected bool IsPhysicsBased { get; set; }

        void Start()
        {
            var zoomControl = new MouseZoomControl<CameraBase>();
            _controls.AddOrSet(zoomControl.Name, zoomControl);
        }

        void FixedUpdate()
        {
            // Execute camera in FixedUpdate() when simulating or interacting
            //   with unity based physics. This allows the camera to leverage the
            //   smoothing built into unity's physics engine.
            // ex: camera follows a ball rolling down a surface.  Unity will
            //   interpolate and smooth in a fixed update function for the simulation.
            //   If the camera doesn't update in FixedUpdate as well, it will jitter.
            // ref: https://forum.unity3d.com/threads/camera-jitter-problem.115224/
            if(IsPhysicsBased)
                OnUpdate(Time.fixedDeltaTime);

        }

        void LateUpdate()
        {
            // Execute camera in LateUpdate() to avoid jitter in due to
            //   the uncontrolled update order of game objects.
            // ex: camera updates, then followed object moves (off by 1 error)
            // ref: https://forum.unity3d.com/threads/camera-jitter-problem.115224/
            if (!IsPhysicsBased)
                OnUpdate(Time.deltaTime);
        }

        protected virtual void OnUpdate(float delta)
        {
            foreach (var control in _controls.Values)
            {
                control.Execute(this, delta);
            }
        }
    }
}