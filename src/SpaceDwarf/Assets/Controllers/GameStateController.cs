using Assets.Framework;
using Assets.Model;
using UnityEngine;

namespace Assets.Controllers
{
    public class GameStateController : SingletonBehavior<GameStateController>
    {
        public enum GameState { Default, Selection }
        private StateManager<GameState> _stateManager;
        
        public enum GlobalState { Default, Pause }
        private StateManager<GlobalState> _globalStateManager;

        void Start()
        {
            // todo: inject state factories
            _stateManager = new StateManager<GameState>(GameState.Default);
            _globalStateManager = new StateManager<GlobalState>(GlobalState.Default);

            // hook events
            _stateManager.OnStateChanged += OnStateChanged;
            _globalStateManager.OnStateChanged += OnGlobalStateChanged;
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            _globalStateManager.CurrentState.Execute(delta);
            _stateManager.CurrentState.Execute(delta);
        }

        public bool ChangeState(GameState newState)
        {
            return _stateManager.ChangeState(newState);
        }

        public bool ChangeGlobalState(GlobalState newState)
        {
            return _globalStateManager.ChangeState(newState);
        }

        private void OnStateChanged(GameState oldState, GameState newState)
        {
            Debug.Log(string.Format("Changed state from {0} to {1}.", oldState, newState));
        }

        private void OnGlobalStateChanged(GlobalState oldState, GlobalState newState)
        {
            Debug.Log(string.Format("Changed Global state from {0} to {1}.", oldState, newState));
        }
    }
}
