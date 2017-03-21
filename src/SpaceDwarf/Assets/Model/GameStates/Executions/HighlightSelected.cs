using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Highlight Selected")]
    public class HighlightSelected : Execution
    {
        public Material HighlightMaterial;
        public override void Execute(StateMachine machine, float timeDelta)
        {
            if (HighlightMaterial == null)
            {
                Debug.LogWarning("Highlight Material was null. Make sure it is set in the inspector.");
                return;
            }

            if (SelectionController.Instance.SelectedObject == null)
            {
                Debug.LogWarning("Attempted to highlight null selected object.");
                return;
            }

            var renderer = SelectionController.Instance.SelectedObject.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogWarning("Could not find sprite renderer on selected object. Cannot highlight.");
                return;
            }

            if(renderer.material != HighlightMaterial)
                renderer.material = HighlightMaterial;
        }
    }
}