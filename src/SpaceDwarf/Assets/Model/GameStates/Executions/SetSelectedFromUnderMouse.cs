using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Set Selected Under Mouse")]
    public class SetSelectedFromUnderMouse : Execution
    {
        public override void Execute(StateMachine machine, float timeDelta)
        {
            // sets object under mouse as Selected Object
            var underMouse = MouseController.Instance.UnderMouse;
            if (underMouse == null)
            {
                Debug.LogWarning("Did not find any selectable items under mouse.");
                return;
            }

            SelectionController.Instance.SetSelected(underMouse);
        }
    }
}