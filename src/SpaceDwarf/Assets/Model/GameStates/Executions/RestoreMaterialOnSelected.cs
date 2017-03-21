using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Restore Material on Selected")]
    public class RestoreMaterialOnSelected : Execution
    {
        public Material OriginalMaterial;

        public override void Execute(StateMachine machine, float timeDelta)
        {
            var go = SelectionController.Instance.SelectedObject;
            if (go == null)
            {
                // nothing selected, restore nothing
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