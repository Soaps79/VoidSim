using Assets.Framework;
using UnityEngine;

namespace Assets.Controllers.Cameras
{
    //  ref: http://gafferongames.com/game-physics/spring-physics/
    //  ref: http://xbox.create.msdn.com/en-US/education/catalog/sample/chasecamera
    public class SpringCamera : FollowCamera
    {
        private Vector3 _prevPosition;

        /// <summary>
        /// Simulated mass of the camera
        /// </summary>
        [Tooltip("Simulated mass of the camera.")]
        public float Mass = 5;

        /// <summary>
        /// Coefficient of Damping. Larger values come to rest more quickly.
        /// </summary>
        [Tooltip("Coefficient of Damping. Larger values come to rest more quickly.")]
        public float Damping = 2;

        /// <summary>
        /// Stiffness constant. Larget values strech less per unit of force.
        /// </summary>
        [Tooltip("Stiffness constant.  Larger values strech less per unit of force.")]
        public float Stiffness = 1;

        [Tooltip("Scalar to apply to simulation.")]
        public float Scalar = 10;

        /// <summary>
        /// Relative velocity between Camera and tracked object
        ///  </summary>
        protected Vector3 Velocity { get; set; }

        protected override void OnStart()
        {
            base.OnStart();

            Velocity = default(Vector3);
            _prevPosition = OffsetCameraPosition;
        }

        /// <summary>
        /// Applies spring physics to Camera position
        /// </summary>
        protected override void OnUpdate(float delta)
        {
            
            base.OnUpdate(delta);

            // scale time factor to increase forces
            delta *= Scalar;
            
            // desired position is where the attached controls and 
            // parent behavior would place us after updating
            var desiredPosition = transform.position;

            // calculate spring force
            // F = -kx - bv
            var stretch = _prevPosition - desiredPosition;
            var force = -Stiffness * stretch - Damping * Velocity;

            // apply acceleration
            var acceleration = force / Mass;
            Velocity += acceleration * delta;

            //Debug.Log(string.Format("dPos{0}, stretch{1}, force{2}, Velocity{3}",
            //    desiredPosition,
            //    stretch,
            //    force,
            //    Velocity));

            // apply velocity
            var position = _prevPosition + Velocity * delta;
            transform.position = position;

            // save for next frame
            _prevPosition = transform.position;
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            var formatted = string.Format("SpringCamera - {0} [LookAt:{1}, Velocity:{2}]\n{3}",
                name,
                LookAt.transform.position.ToVector2(),
                Velocity.ToVector2(),
                baseStr);

            return formatted;
        }
    }
}
