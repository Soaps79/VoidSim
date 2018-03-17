using System;
using UnityEngine;

namespace QGame
{
    /// <summary>
    /// Wraps a MonoBehavior, adding in time modification functionality and useful update callbacks
    /// </summary>
    public abstract class OrderedEventBehavior : MonoBehaviour
    {
        /// <summary>
        /// Static variable that controls time modification for all QScripts
        /// </summary>
        public static float TimeModifier { get; set; } = 1.0f;

        /// <summary>
        /// Used by any QScript to opt out of using time modification
        /// </summary>
        public bool UseTimeModifier { get; set; } = true;

        public Action OnEveryUpdate;
        public Action OnNextUpdate;

        /// <summary>
        /// Used to clear OnEveryUpdate and OnNextUpdate
        /// </summary>
        protected virtual void ClearUpdateCallbacks()
        {
            OnNextUpdate = null;
            OnEveryUpdate = null;
        }

        /// <summary>
        /// Triggers the update cycle, do not override in subclasses, exposed for testing purposes only
        /// </summary>
        protected void Update()
        {
            HandleUpdate();
        }

        private void HandleUpdate()
        {
            OnUpdateStart();
            UpdateCallbacks();
            OnUpdate();
            OnUpdateEnd();
        }

        /// <summary>
        /// The first function called in an Update cycle (before OnEveryUpdate and OnNextUpdate)
        /// </summary>
        protected virtual void OnUpdateStart() { }
        /// <summary>
        /// The second function called in an Update cycle, just after OnEveryUpdate and OnNextUpdate
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// The final function called in an Update cycle
        /// </summary>
        protected virtual void OnUpdateEnd() { }

        protected virtual void UpdateCallbacks()
        {
            if (OnNextUpdate != null)
            {
				// current actions held in new list so that others can be added in one of these calls
	            var next = OnNextUpdate;
	            OnNextUpdate = null;
                next();
            }

            if (OnEveryUpdate != null)
            {
                OnEveryUpdate();
            }
        }

        /// <summary>
        /// Returns delta time, factoring in time modification
        /// </summary>
        protected float GetDelta()
        {
            return UseTimeModifier ? TimeModifier * Time.deltaTime : Time.deltaTime;
        }
    }
}