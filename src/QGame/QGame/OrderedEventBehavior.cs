using System;
using UnityEngine;

namespace QGame
{
    public abstract class OrderedEventBehavior : MonoBehaviour
    {
        public static float TimeModifier { get; set; } = 1.0f;
        public bool UseTimeModifier { get; set; } = true;

        public Action OnEveryUpdate;
        public Action OnNextUpdate;

        public virtual void ClearAllDelegates()
        {
            OnNextUpdate = null;
            OnEveryUpdate = null;
        }

        void Update()
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

        protected virtual void OnUpdateStart() { }
        protected virtual void OnUpdate() { }
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

        protected float GetDelta()
        {
            return UseTimeModifier ? TimeModifier * Time.deltaTime : Time.deltaTime;
        }
    }
}