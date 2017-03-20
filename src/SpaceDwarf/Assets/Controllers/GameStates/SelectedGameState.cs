using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class SelectedGameState : State<GameModel>
    {
        public override string Name { get { return "SelectedGameState"; } }
        
        private Material _originalMaterial;
        private readonly Material _selectedMaterial;
        public SelectedGameState(Material selectedMaterial)
        {
            _selectedMaterial = selectedMaterial;
            SelectionController.Instance.OnSelectedChanged += OnSelectedChangedHandler;
        }

        private void OnSelectedChangedHandler(GameObject prev, GameObject next)
        {
            // reset previous material
            var prevRenderer = GetRendererFromObject(prev);
            if (_originalMaterial != null)
            {
                if (prevRenderer != null && prevRenderer.material != null
                    && prevRenderer.material.name != _originalMaterial.name)
                {
                    prevRenderer.material = _originalMaterial;
                }
            }
            else
            {
                Debug.LogWarning("Unknown original material, cannot unset selected!");
            }

            var renderer = GetRendererFromObject(next);
            if (renderer == null)
            {
                // nothing to see, bail
                Debug.LogWarning("Couldn't find renderer on Selected object.");
                return;
            }

            // set original material
            _originalMaterial = renderer.material;

            // set selected material
            renderer.material = _selectedMaterial;
        }

        public void SetSelectedObject(GameObject gameObject, Material originalMaterial = null)
        {
            // set selected
            SelectionController.Instance.SetSelected(gameObject);

            // if the original material is provided, we need to set it before interaction
            if (originalMaterial != null)
            {
                // replace whatever we've got with the provided material
                _originalMaterial = originalMaterial;
            }
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            // check for transitions
            if (Input.GetButtonDown("SelectionMode") || Input.GetMouseButtonDown(1))
            {
                // revert to whatever, probably selection state
                Machine.Revert();
            }

            if (Input.GetButtonDown("Cancel"))
            {
                // exit to DefaultGameState
                Machine.ChangeState(GameStateController.Instance.DefaultGameState);
            }
        }

        public override void Exit(GameModel owner)
        {
            base.Exit(owner);

            // reset texture
            var renderer = GetRendererFromObject(SelectionController.Instance.SelectedObject);
            if (renderer == null)
            {
                return;
            }

            renderer.material = _originalMaterial;
            SelectionController.Instance.SetSelected(null);
            _originalMaterial = null;
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
