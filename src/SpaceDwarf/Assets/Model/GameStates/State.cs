using System;
using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates
{
    [CreateAssetMenu(menuName = "GameStates/State")]
    [Serializable]
    public class State : ScriptableObject
    {
        public Execution[] OnEnterExecutions;
        public Execution[] OnExitExecutions;
        public Execution[] Executions;
        public Transition[] Transitions;
        
        public Action<State, StateMachine> OnEnter = null;
        public Action<State, StateMachine, float> OnExecute = null;
        public Action<State, StateMachine> OnExit = null;
        
        void OnEnable()
        {
            OnEnabled();
        }

        protected virtual void OnEnabled() { }
        
        public virtual void Enter(StateMachine machine)
        {
            DoEnterExecutions(machine);
            if (OnEnter != null)
                OnEnter(this, machine);
        }

        private void DoEnterExecutions(StateMachine machine)
        {
            for(var i = 0; i < OnEnterExecutions.Length; i++)
            {
                OnEnterExecutions[i].Execute(machine, Time.deltaTime);
            }
        }

        public virtual void Execute(StateMachine machine, float timeDelta)
        {
            DoExecutions(machine, timeDelta);
            if (OnExecute != null)
                OnExecute(this, machine, timeDelta);

            DoTransitions(machine, timeDelta);
        }

        private void DoExecutions(StateMachine machine, float timeDelta)
        {
            for(var i = 0; i < Executions.Length; i++)
            {
                Executions[i].Execute(machine, timeDelta);
            }
        }

        private void DoTransitions(StateMachine machine, float timeDelta)
        {
            for(var i = 0; i < Transitions.Length; i++)
            {
                var transition = Transitions[i];
                if (transition.Decision == null)
                {
                    Debug.LogWarning("A transition's decision was null. Make sure it's set up in the inspector.");
                    continue;
                }
                var decision = transition.Decision.Decide(machine, timeDelta);

                machine.ChangeState(
                    decision ? 
                    transition.TrueState :
                    transition.FalseState);
            }
        }

        public virtual void Exit(StateMachine machine)
        {
            DoExitExecutions(machine);
            if (OnExit != null)
                OnExit(this, machine);
        }

        private void DoExitExecutions(StateMachine machine)
        {
            for(var i = 0; i < OnExitExecutions.Length; i++)
            {
                OnExitExecutions[i].Execute(machine, Time.deltaTime);
            }
        }
        
        public virtual bool CanTransition(State nextState)
        {
            if (nextState == null)
            {
                Debug.LogWarning("Tried to transition into null state");
                return false;
            }
            return true;
        }
    }
}