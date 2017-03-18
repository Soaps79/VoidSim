using System;
using UnityEngine;

namespace Assets.Model
{
    public abstract class State<T>
    {
        public abstract string Name { get; }

        public Action<State<T>, T> OnEnter = null;
        public Action<State<T>, T, float> OnExecute = null;
        public Action<State<T>, T> OnExit = null;

        protected StateMachine<T> Machine { get; private set; }

        public void RegisterMachine(StateMachine<T> machine)
        {
            Machine = machine;
        }
        
        public virtual void Enter(T owner)
        {
            if (OnEnter != null)
                OnEnter(this, owner);
        }

        public virtual void Execute(T owner, float timeDelta)
        {
            if (OnExecute != null)
                OnExecute(this, owner, timeDelta);
        }

        public virtual void Exit(T owner)
        {
            if (OnExit != null)
                OnExit(this, owner);
        }
        
        public virtual bool CanTransition(State<T> nextState)
        {
            if (nextState == null)
            {
                Debug.LogWarning("Tried to transition into null state");
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}