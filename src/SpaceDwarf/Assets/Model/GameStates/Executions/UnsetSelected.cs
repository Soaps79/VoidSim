using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Unset Selected")]
    public class UnsetSelected : Execution
    {
        public override void Execute(StateMachine machine, float timeDelta)
        {
            SelectionController.Instance.Clear();
        }
    }
}