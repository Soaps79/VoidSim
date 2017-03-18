using Assets.Model;
using UnityEngine;

namespace Assets.Controllers
{
    // global states
    public class DefaultGlobalState : State<GameModel>
    {
        public override string Name { get { return "DefaultGlobalState"; } }

        private readonly GameStateController _stateController;

        public DefaultGlobalState(
            GameStateController stateController)
        {
            _stateController = stateController;
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            if (Input.GetButtonDown("Pause"))
            {
                _stateController.ChangeGlobalState(_stateController.PauseGameState);
            }
        }
    }

    public class PauseGameState : State<GameModel>
    {
        public override string Name { get { return "PauseGameState"; } }

        private readonly TerrainController _terrainController;
        private readonly PlayerController _playerController;
        private readonly GameStateController _stateController;

        public PauseGameState(
            TerrainController terrainController, 
            PlayerController playerController,
            GameStateController stateController)
        {
            _terrainController = terrainController;
            _playerController = playerController;
            _stateController = stateController;
        }

        public override void Enter(GameModel owner)
        {
            base.Enter(owner);
            Pause();
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            if (Input.GetButtonDown("Pause"))
            {
                _stateController.RevertGlobalState();
            }
        }

        public override void Exit(GameModel owner)
        {
            base.Exit(owner);
            Unpause();
        }

        private void Pause()
        {
            // pause objects
            _terrainController.enabled = false;
            _playerController.enabled = false;
        }

        private void Unpause()
        {
            // unpause objects
            _terrainController.enabled = true;
            _playerController.enabled = true;
        }
    }

    // Game States
    public class DefaultGameState : State<GameModel>
    {
        public override string Name { get { return "DefaultGameState"; } }
    }
}
