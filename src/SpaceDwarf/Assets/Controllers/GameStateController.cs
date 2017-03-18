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

        // global states
        [Inject] public DefaultGlobalState DefaultGlobalState;
        [Inject] public PauseGameState PauseGameState;

        // game states
        [Inject] public DefaultGameState DefaultGameState;

        void Start()
        {
            // todo: inject state factories

            _stateMachine = new StateMachine<GameModel>(GameModel, DefaultGameState);
            _globalStateMachine = new StateMachine<GameModel>(GameModel, DefaultGlobalState);
            _globalStateMachine.AddState(PauseGameState.Name, PauseGameState);

            // hook events
            _stateMachine.OnStateChanged += OnStateChanged;
            _globalStateMachine.OnStateChanged += OnGlobalStateChanged;
        }

        public override void OnUpdate(float delta)
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

        private void OnStateChanged(State<GameModel> oldState, State<GameModel> newState)
        {
            Debug.Log(string.Format("Changed state from {0} to {1}.", oldState, newState));
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
    }
}
