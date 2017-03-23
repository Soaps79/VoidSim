﻿using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Pause Controllers")]
    public class PauseControllers : Execution
    {
        public bool IsPause = true;
        public override void Execute(StateMachine machine, float timeDelta)
        {
            
            //TerrainController.Instance.enabled = false;
            //PlayerController.Instance.enabled = false;
            CameraController.Instance.enabled = !IsPause;
        }
    }
}
