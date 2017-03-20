﻿using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class SelectionGameState : State<GameModel>
    {
        public override string Name { get { return "SelectionGameState"; } }

        private readonly PlayerCharacter _character;

        private Material _originalMaterial;
        private readonly Material _selectionMaterial;

        private GameObject _previousUnderMouse;
        

        public SelectionGameState(
            PlayerCharacter character,
            Material selectionMaterial)
        {
            _character = character;
            _selectionMaterial = selectionMaterial;
        }

        public override void Enter(GameModel owner)
        {
            base.Enter(owner);

            // change camera to Pan and Scan

            // disable player movement
            _character.CanMove = false;
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            // check for transitions
            if (Input.GetButtonDown("SelectionMode"))
            {
                // cancelling selection mode
                Machine.Revert();
                Reset();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // clicked, change to selected state
                Machine.ChangeState(GameStateController.Instance.SelectedGameState);

                GameStateController.Instance.SelectedGameState.SetSelectedObject(
                    MouseController.Instance.UnderMouse, _originalMaterial);
                Reset();
                return;
            }

            // get object under mouse
            var underMouse = MouseController.Instance.UnderMouse;
            if (underMouse == null) { return; }
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
                prevRenderer.material = _originalMaterial;
            }

            // set selection material
            var renderer = GetRendererFromObject(underMouse);
            var currentMaterial = renderer.material;
            if (currentMaterial.name == _selectionMaterial.name)
            {
                // already applied selection material
                return;
            }

            // save current material as original
            _originalMaterial = renderer.material;
            renderer.material = _selectionMaterial;

            // save for next frame
            _previousUnderMouse = underMouse;
        }

        private void Reset()
        {
            _originalMaterial = null;
            _previousUnderMouse = null;
        }

        public override void Exit(GameModel owner)
        {
            base.Exit(owner);

            // restore original material
            RestoreOriginalMaterial();

            // change camera back to previous

            // enable player movement
            _character.CanMove = true;
        }

        private void RestoreOriginalMaterial()
        {
            var renderer = GetRendererUnderMouse();
            if (_originalMaterial != null)
                renderer.material = _originalMaterial;

            _originalMaterial = null;
        }

        private SpriteRenderer GetRendererUnderMouse()
        {
            // find game object
            // get item under cursor
            var go = MouseController.Instance.UnderMouse;
            if (go == null)
            {
                return null;
            }

            return GetRendererFromObject(go);
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