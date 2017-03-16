using System;
using System.Collections.Generic;
using Assets.Framework;
using UnityEngine;

namespace Assets.Model
{
    public class StateManager<T> 
    {
        private T _currentState;
        private T _previousState;

        public Action<T, T> OnStateChanged;

        private readonly Dictionary<T, State<T>> _states = 
            new Dictionary<T, State<T>>();
        
        public State<T> CurrentState { get { return GetStateFromType(_currentState); } }

        public StateManager(T state)
        {
            AddState(state, new State<T>(state));

            // set current and previous on creation
            _currentState = _previousState = state;
        }

        public bool ChangeState(T newState)
        {
            var nextState = GetStateFromType(newState);
            if(nextState == null) {  return false; }
            
            // escape if we can't transition
            if (!CurrentState.CanTransition(nextState))
            {
                Debug.Log(string.Format("Could not transition from {0} to {1}.", _currentState, newState));
                return false;
            }

            // change states
            CurrentState.Exit();
            _previousState = _currentState;
            _currentState = newState;
            CurrentState.Enter();

            // fire state changed event
            if (OnStateChanged != null)
                OnStateChanged(_previousState, _currentState);

            return true;
        }

        public bool Revert()
        {
            return ChangeState(_previousState);
        }

        public void AddState(T type, State<T> state)
        {
            _states.AddOrSet(type, state);
        }

        public State<T> GetState(T type)
        {
            return GetStateFromType(type);
        }


        public void RegisterOnStateChangedCallback(Action<T, T> callback)
        {
            OnStateChanged += callback;
        }

        private State<T> GetStateFromType(T type)
        {
            if (!_states.ContainsKey(type))
            {
                Debug.LogWarning(string.Format("No state found for key: {0}", type));
                return null;
            }

            return _states[type];
        }
    }
}