using System;
using Assets.Model.GameStates;
using QGame;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    [Serializable]
    public class StateMachine : OrderedEventBehavior
    {
        private State _previousState;
        
        public State CurrentState;
        public State StartingState;
        public State RemainState;

        public Action<State, State> OnStateChanged;

        protected override void OnStart()
        {
            // set current and previous on creation
            CurrentState = _previousState = StartingState;
        }

        protected override void OnUpdate(float timeDelta)
        {
            base.OnUpdate(timeDelta);
            CurrentState.Execute(this, timeDelta);
        }

        public void ChangeState(State nextState)
        {
            if (nextState == RemainState)
            {
                return;
            }

            // escape if we can't transition
            if (!CurrentState.CanTransition(nextState))
            {
                Debug.Log(string.Format("Could not transition from {0} to {1}.", CurrentState, nextState));
                return;
            }

            // change states
            CurrentState.Exit(this);
            SwapStates(nextState);
            CurrentState.Enter(this);

            // fire state changed event
            if (OnStateChanged != null)
                OnStateChanged(_previousState, CurrentState);
        }
        
        private void SwapStates(State nextState)
        {
            _previousState = CurrentState;
            CurrentState = nextState;
        }
    }
}