using Assets.Model;
using QGame;
using UnityEngine;

namespace Assets.Controllers
{
    /// <summary>
    /// Selection Behavior to attach to individual GameObjects.
    /// Note that a better solution exists in SelectionGameState such
    /// that you don't have to apply this behavior to thousands of objects.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SelectionBehavior : OrderedEventBehavior
    {
        public Material SelectionMaterial;

        private Material _originalMaterial = null;
        private SpriteRenderer _spriteRenderer;

        protected override void OnStart()
        {
            base.OnStart();
            IsEnabled = false;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalMaterial = _spriteRenderer.material;

            // hook game state changed event
            GameStateController.Instance.RegisterStateChangeCallback(OnStateChanged);
        }

        private void OnStateChanged(State<GameModel> oldState, State<GameModel> newState)
        {
            IsEnabled = newState.Name == GameStateController.Instance.SelectionGameState.Name;
            if (!IsEnabled)
            {
                // if we're leaving the state, replace with original material
                _spriteRenderer.material = _originalMaterial;
            }
            else
            {
                // if we're entering the state, check to see if this object
                //   is currently under the mouse cursor.
                if (!IsUnderMouse())
                    return;

                // we're under the cursor, swap
                _spriteRenderer.material = SelectionMaterial;
            }
        }

        void OnMouseEnter()
        {
            if (!IsEnabled)
            {
                return;
            }

            // project a ray to see if this is blocked
            if (!IsUnderMouse())
                return;

            // change shader
            _spriteRenderer.material = SelectionMaterial;
        }

        private bool IsUnderMouse()
        {
            var underMouse = MouseController.Instance.UnderMouse;
            return underMouse == gameObject;
        }
        

        void OnMouseOver()
        {
            
        }

        void OnMouseExit()
        {
            if (!IsEnabled)
            {
                return;
            }

            // change shader back
            _spriteRenderer.material = _originalMaterial;
        }


    }
}
