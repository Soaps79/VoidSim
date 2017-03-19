using System;
using UnityEngine;

namespace QGame
{
    public abstract class OrderedEventBehavior : MonoBehaviour
    {
        private bool _isEnabled = true;

        public Action<OrderedEventBehavior> EnableChanged;
        public Action<float> OnEveryUpdate;
        public Action<float> OnNextUpdate;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = TrySet(_isEnabled, value, EnableChanged); }
        }

        public virtual void ClearAllDelegates()
        {
            OnNextUpdate = null;
            OnEveryUpdate = null;
            EnableChanged = null;
        }

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart() { }

        void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake() { }

        void Update()
        {
            Update(Time.deltaTime);
        }

        private void Update(float delta)
        {
            OnUpdateStart(delta);
            OnUpdate(delta);
            OnUpdateEnd(delta);
        }

        protected virtual void OnUpdateStart(float delta) { }
        protected virtual void OnUpdateEnd(float delta) { }
        protected virtual void OnUpdate(float delta)
        {
            if (OnNextUpdate != null)
            {
                OnNextUpdate(delta);
                OnNextUpdate = null;
            }

            if (OnEveryUpdate != null)
            {
                OnEveryUpdate(delta);
            }
        }

        void FixedUpdate()
        {
            FixedUpdate(Time.fixedDeltaTime);
        }

        private void FixedUpdate(float delta)
        {
            OnFixedUpdate(delta);
        }

        protected virtual void OnFixedUpdate(float delta) { }

        void LateUpdate()
        {
            LateUpdate(Time.deltaTime);
        }

        private void LateUpdate(float delta)
        {
            OnLateUpdate(delta);
        }

        protected virtual void OnLateUpdate(float delta) { }

        void OnGUI()
        {
            OnGUIDraw(Time.deltaTime);
        }

        protected virtual void OnGUIDraw(float delta) { }

        /// <summary>
        /// Only fire event if the value is changed.
        /// </summary>
        /// <param name="currentValue">Current value</param>
        /// <param name="newValue">Value to set</param>
        /// <param name="onChangedCallback">Callback to fire if value is changed</param>
        /// <returns>The value to set.</returns>
        protected bool TrySet(bool currentValue, bool newValue, Action<OrderedEventBehavior> onChangedCallback)
        {
            if (currentValue != newValue)
            {
                currentValue = newValue;
                if (onChangedCallback != null)
                {
                    onChangedCallback(this);
                }
            }

            return currentValue;
        }
        
    }
}