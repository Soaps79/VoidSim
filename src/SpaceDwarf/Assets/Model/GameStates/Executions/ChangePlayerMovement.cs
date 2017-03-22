using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Change Player Movement")]
    public class ChangePlayerMovement : Execution
    {
        public bool CanMove = true;

        public override void Execute(StateMachine machine, float timeDelta)
        {
            PlayerController.Instance.PlayerCharacter.CanMove = CanMove;
        }
    }
}
