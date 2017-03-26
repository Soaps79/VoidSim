using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Controllers;
using Assets.Controllers.Player;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Unpause Controllers")]
    public class UnpauseControllers : Execution
    {
        public override void Execute(StateMachine machine, float timeDelta)
        {
            TerrainController.Instance.enabled = true;
            PlayerController.Instance.enabled = true;
            CameraController.Instance.enabled = true;
        }
    }
}
