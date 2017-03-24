using System;
using Assets.Controllers.GameStates;
using UnityEngine;

namespace Assets.Model.GameStates
{
    /// <summary>
    /// Abstract conditional to control State Machine <see cref="Transition"/>.
    /// </summary>
    [Serializable]
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateMachine owner, float timeDelta);
    }
}
