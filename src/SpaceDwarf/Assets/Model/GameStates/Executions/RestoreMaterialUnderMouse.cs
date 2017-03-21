using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Restore Material Under Mouse")]
    public class RestoreMaterialUnderMouse : Execution
    {
        public Material OriginalMaterial;

        public override void Execute(StateMachine machine, float timeDelta)
        {
            var go = MouseController.Instance.UnderMouse;
            if (go == null)
            {
                // nothing under cursor, restore nothing
                return;
            }
            
            var renderer = go.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogWarning(string.Format("No sprite renderer found on {0}", go));
                return;
            }
            Debug.Log(string.Format("Restoring Original Material on {0}", go.name));
            renderer.material = OriginalMaterial;
        }
    }
}
