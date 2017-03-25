using System.Collections.Generic;
using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Change Selectable Layers")]
    public class SetCanSelectLayers : Execution
    {
        public bool CanSelect = true;
        public List<string> Layers;

        public override void Execute(StateMachine machine, float timeDelta)
        {
            for (var i = 0; i < Layers.Count; i++)
            {
                SelectionController.Instance.SetCanSelect(Layers[i], CanSelect);
            }
        }
    }
}
