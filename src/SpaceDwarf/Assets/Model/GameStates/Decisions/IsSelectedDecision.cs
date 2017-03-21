using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Decisions
{
    [CreateAssetMenu(menuName = "GameStates/Decisions/Is Selected")]
    public class IsSelectedDecision : Decision
    {
        public int ButtonNumber = 0;
        public override bool Decide(StateMachine owner, float timeDelta)
        {
            var btnDown = Input.GetMouseButtonDown(ButtonNumber);
            if (!btnDown)
                return false;

            var isSelected = MouseController.Instance.UnderMouse != null;
            //if(isSelected)
            //    Debug.Log("Selected!");

            return isSelected;
        }
    }
}
