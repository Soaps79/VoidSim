using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class PauseGameState : State<GameModel>
    {
        public override string Name { get { return "PauseGameState"; } }

        private readonly TerrainController _terrainController;
        private readonly PlayerController _playerController;

        public PauseGameState(
            TerrainController terrainController, 
            PlayerController playerController)
        {
            _terrainController = terrainController;
            _playerController = playerController;
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
                Machine.Revert();
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
}