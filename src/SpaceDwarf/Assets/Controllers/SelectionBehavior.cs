using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Model;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Controllers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SelectionBehavior : OrderedEventBehavior
    {
        // todo: centralize
        private const int PlayerLayerMask = 1 << 11;
        private const int UnitsLayerMask = 1 << 10;
        private const int BuildingsLayerMask = 1 << 9;
        private const int TerrainLayerMask = 1 << 8;

        public Material SelectionMaterial;

        //todo: solve injection issues
        // weirdness, sometimes null.
        //[Inject] public GameStateController GameStateController;
        //[Inject] public CameraController CameraController { get; set; }

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
            var activeCamera = CameraController.Instance.ActiveCamera;
            var underMouse = GetObjectUnderMouse(activeCamera);
            return underMouse == gameObject;
        }

        private GameObject GetObjectUnderMouse(Camera activeCamera)
        {
            // project ray from screen into world
            
            var ray = activeCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            GameObject foundGo = null;

            // use layers to avoid z-fighting
            // Player -> Units -> Buildings -> Terrain
            if (Physics.Raycast(ray, out hit, 100, PlayerLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, UnitsLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, BuildingsLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100, TerrainLayerMask))
            {
                foundGo = hit.transform.gameObject;
            }

            return foundGo;
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
