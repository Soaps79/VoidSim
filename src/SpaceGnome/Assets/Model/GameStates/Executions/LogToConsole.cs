using System;
using Assets.Controllers.GameStates;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [Serializable]
    [CreateAssetMenu(menuName = "GameStates/Executions/Log to Console")]
    public class LogToConsole : Execution
    {
        public string Message = "Hello World";
        public override void Execute(StateMachine machine, float timeDelta)
        {
            Debug.Log("LogToConsole: " + Message);
        }
    }
}
