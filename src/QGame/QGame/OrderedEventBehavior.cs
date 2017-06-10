﻿using System;
using UnityEngine;

namespace QGame
{
    public abstract class OrderedEventBehavior : MonoBehaviour
    {
        public static float TimeModifier { get; set; } = 1.0f;
        public bool UseTimeModifier { get; set; } = true;

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
            if(_isEnabled)
                Update(Time.deltaTime);
        }

        private void Update(float delta)
        {
            if(UseTimeModifier) delta =  delta*TimeModifier;
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
				// current actions held in new list so that others can be added in one of these calls
	            var next = OnNextUpdate;
	            OnNextUpdate = null;
                next(delta);
            }

            if (OnEveryUpdate != null)
            {
                OnEveryUpdate(delta);
            }
        }

        void FixedUpdate()
        {
            if(_isEnabled)
                FixedUpdate(Time.fixedDeltaTime);
        }

        private void FixedUpdate(float delta)
        {
            OnFixedUpdate(delta);
        }

        protected virtual void OnFixedUpdate(float delta) { }

        void LateUpdate()
        {
            if(IsEnabled)
                LateUpdate(Time.deltaTime);
        }

        private void LateUpdate(float delta)
        {
            OnLateUpdate(delta);
        }

        protected virtual void OnLateUpdate(float delta) { }

        void OnGUI()
        {
            if(IsEnabled)
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