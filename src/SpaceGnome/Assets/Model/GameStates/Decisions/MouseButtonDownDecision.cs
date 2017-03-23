using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Decisions
{
    [CreateAssetMenu(menuName = "GameStates/Decisions/Mouse Button Down")]
    public class MouseButtonDownDecision : Decision
    {
        public int ButtonNumber = 0;
        public override bool Decide(StateMachine owner, float timeDelta)
        {
            var btnDown = Input.GetMouseButtonDown(ButtonNumber);
            return btnDown;
        }
    }
}
