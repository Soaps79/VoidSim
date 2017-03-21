using System;
using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates
{
    [Serializable]
    public abstract class Execution : ScriptableObject
    {
        public abstract void Execute(StateMachine machine, float timeDelta);
    }
}
