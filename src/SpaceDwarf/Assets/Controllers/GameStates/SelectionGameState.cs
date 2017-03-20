using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class SelectionGameState : State<GameModel>
    {
        public override string Name { get { return "SelectionGameState"; } }

        private readonly PlayerCharacter _character;
        private readonly CameraController _cameraController;

        public SelectionGameState(
            PlayerCharacter character,
            CameraController cameraController)
        {
            _character = character;
            _cameraController = cameraController;
        }

        public override void Enter(GameModel owner)
        {
            base.Enter(owner);

            // change camera to Pan and Scan

            // disable player movement
            _character.CanMove = false;
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);
            
            // check for transitions
            if (Input.GetButtonDown("SelectionMode"))
            {
                Machine.Revert();
            }
        }

        public override void Exit(GameModel owner)
        {
            base.Exit(owner);

            // change camera back to previous

            // enable player movement
            _character.CanMove = true;
        }
    }
}
