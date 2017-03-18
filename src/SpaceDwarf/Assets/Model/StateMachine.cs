using System;
using System.Collections.Generic;
using Assets.Framework;
using UnityEngine;

namespace Assets.Model
{
    public class StateMachine<T>
    {
        private readonly T _owner;
        private State<T> _previousState;

        public Action<State<T>, State<T>> OnStateChanged;

        private readonly Dictionary<string, State<T>> _states = 
            new Dictionary<string, State<T>>();

        public State<T> CurrentState { get; private set; }

        public StateMachine(T owner, State<T> initialState)
        {
            _owner = owner;
            AddState(initialState.Name, initialState);

            // set current and previous on creation
            CurrentState = _previousState = initialState;
        }

        public bool ChangeState(State<T> nextState)
        {
            // escape if we can't transition
            if (!CurrentState.CanTransition(nextState))
            {
                Debug.Log(string.Format("Could not transition from {0} to {1}.", CurrentState, nextState));
                return false;
            }

            // change states
            CurrentState.Exit(_owner);
            SwapStates(nextState);
            CurrentState.Enter(_owner);

            // fire state changed event
            if (OnStateChanged != null)
                OnStateChanged(_previousState, CurrentState);

            return true;
        }
        
        private void SwapStates(State<T> nextState)
        {
            _previousState = CurrentState;
            CurrentState = nextState;
        }

        public bool Revert()
        {
            return ChangeState(_previousState);
        }

        public void AddState(string key, State<T> state)
        {
            _states.AddOrSet(key, state);
            state.RegisterMachine(this);
        }
    }
}