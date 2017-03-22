using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Change Active Camera")]
    public class ChangeActiveCamera : Execution
    {
        public string CameraName = "";
        public override void Execute(StateMachine machine, float timeDelta)
        {
            if (string.IsNullOrEmpty(CameraName))
            {
                Debug.LogWarning("CameraName to activate/deactive not set. Make sure you set it in the inspector.");
            }
            CameraController.Instance.ChangeCamera(CameraName);
        }
    }
}
