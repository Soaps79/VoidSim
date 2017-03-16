using System;

namespace Assets.Model
{
    public class State<T>
    {
        private readonly T _type;
        public T Type { get { return _type; } }

        public Action<State<T>> OnEnter = null;
        public Action<State<T>> OnExecute = null;
        public Action<State<T>> OnExit = null;

        public State(T type)
        {
            _type = type;
        }

        public virtual void Enter()
        {
            if (OnEnter != null)
                OnEnter(this);
        }

        public virtual void Execute(float timeDelta)
        {
            if (OnExecute != null)
                OnExecute(this);
        }

        public virtual void Exit()
        {
            if (OnExit != null)
                OnExit(this);
        }
        
        public virtual bool CanTransition(State<T> nextState)
        {
            return true;
        }
    }
}