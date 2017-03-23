using System;

namespace Assets.Model.GameStates
{
    [Serializable]
    public class Transition
    {
        public Decision Decision;
        public State TrueState;
        public State FalseState;
    }
}
