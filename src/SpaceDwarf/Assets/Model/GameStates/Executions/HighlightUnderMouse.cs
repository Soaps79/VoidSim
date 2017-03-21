using Assets.Controllers;
using UnityEngine;

namespace Assets.Model.GameStates.Executions
{
    [CreateAssetMenu(menuName = "GameStates/Executions/Highlight Under Mouse")]
    public class HighlightUnderMouse : Execution
    {
        public Material SelectionMaterial;

        private Material _originalMaterial;
        private GameObject _previousUnderMouse;
        public override void Execute(StateMachine owner, float timeDelta)
        {
            HighlightObjectUnderMouse(timeDelta);
        }

        private void HighlightObjectUnderMouse(float timeDelta)
        {
            // get object under mouse
            var underMouse = MouseController.Instance.UnderMouse;
            if (underMouse == null)
            {
                return;
            }
            if (_previousUnderMouse != null
                && underMouse.name == _previousUnderMouse.name)
            {
                // same object as previous, all done here
                return;
            }

            // it's different, unswap previous item material
            if (_previousUnderMouse != null)
            {
                var prevRenderer = GetRendererFromObject(_previousUnderMouse);
                if (_originalMaterial != null) 
                    prevRenderer.material = _originalMaterial;
            }

            // set selection material
            var renderer = GetRendererFromObject(underMouse);
            var currentMaterial = renderer.material;
            if (currentMaterial.name == SelectionMaterial.name)
            {
                // already applied selection material
                return;
            }

            // save current material as original
            _originalMaterial = renderer.material;
            renderer.material = SelectionMaterial;

            // save for next frame
            _previousUnderMouse = underMouse;
        }

        private static SpriteRenderer GetRendererFromObject(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            var renderer = go.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogWarning(string.Format(
                    "SpriteRenderer not available on {0}, could not apply select material.",
                    go.name));
                return null;
            }

            return renderer;
        }
    }
}
