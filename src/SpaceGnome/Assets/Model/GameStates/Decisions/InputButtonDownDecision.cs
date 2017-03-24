using Assets.Controllers.GameStates;
using UnityEngine;

namespace Assets.Model.GameStates.Decisions
{
    [CreateAssetMenu(menuName = "GameStates/Decisions/Input Button Down")]
    public class InputButtonDownDecision : Decision
    {
        public string ButtonName;
        public override bool Decide(StateMachine owner, float timeDelta)
        {
            if (string.IsNullOrEmpty(ButtonName))
            {
                Debug.LogWarning("Must provide the button name to check in InputButtonDownDecision. " +
                                 "Did you forget to set it in the editor?");
                return true;
            }
            var btnDown = Input.GetButtonDown(ButtonName);
            return btnDown;
        }
    }
}
