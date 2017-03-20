using System;
using Assets.Controllers.GameStates;
using Assets.Framework;
using Assets.Model;
using UnityEngine;
using Zenject;

namespace Assets.Controllers
{
    public class GameStateController : SingletonBehavior<GameStateController>
    {
        [Inject]
        public GameModel GameModel;

        private StateMachine<GameModel> _stateMachine;
        private StateMachine<GameModel> _globalStateMachine;

        private Action<State<GameModel>, State<GameModel>> _onStateChanged;
        private Action<State<GameModel>, State<GameModel>> _onGlobalStateChanged;

        // global states
        [Inject] public DefaultGlobalState DefaultGlobalState;
        [Inject] public PauseGameState PauseGameState;

        // game states
        [Inject] public DefaultGameState DefaultGameState;
        [Inject] public SelectionGameState SelectionGameState;
        [Inject] public SelectedGameState SelectedGameState;

        protected override void OnStart()
        {
            base.OnStart();

            // todo: inject state factories

            _stateMachine = new StateMachine<GameModel>(GameModel, DefaultGameState);
            _stateMachine.AddState(SelectionGameState.Name, SelectionGameState);
            _stateMachine.AddState(SelectedGameState.Name, SelectedGameState);

            _globalStateMachine = new StateMachine<GameModel>(GameModel, DefaultGlobalState);
            _globalStateMachine.AddState(PauseGameState.Name, PauseGameState);

            // hook events
            _stateMachine.OnStateChanged += OnStateChangedHandler;
            _globalStateMachine.OnStateChanged += OnGlobalStateChanged;
        }

        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            _globalStateMachine.CurrentState.Execute(GameModel, delta);
            _stateMachine.CurrentState.Execute(GameModel, delta);
        }

        public bool ChangeState(State<GameModel> newState)
        {
            return _stateMachine.ChangeState(newState);
        }

        public bool ChangeGlobalState(State<GameModel> newState)
        {
            return _globalStateMachine.ChangeState(newState);
        }

        private void OnStateChangedHandler(State<GameModel> oldState, State<GameModel> newState)
        {
            Debug.Log(string.Format("Changed state from {0} to {1}.", oldState, newState));
            if (_onStateChanged != null)
                _onStateChanged(oldState, newState);
        }

        private void OnGlobalStateChanged(State<GameModel> oldState, State<GameModel> newState)
        {
            Debug.Log(string.Format("Changed Global state from {0} to {1}.", oldState, newState));
        }

        public void RevertState()
        {
            _stateMachine.Revert();
        }

        public void RevertGlobalState()
        {
            _globalStateMachine.Revert();
        }

        public void RegisterStateChangeCallback(Action<State<GameModel>, State<GameModel>> callback)
        {
            _onStateChanged += callback;
        }

        public void RegisterGlobalStateChangeCallback(Action<State<GameModel>, State<GameModel>> callback)
        {
            _onGlobalStateChanged += callback;
        }
    }
}
