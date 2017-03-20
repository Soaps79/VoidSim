using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class SelectionGameState : State<GameModel>
    {
        // todo: centralize
        private const int PlayerLayerMask = 1 << 11;
        private const int UnitsLayerMask = 1 << 10;
        private const int BuildingsLayerMask = 1 << 9;
        private const int TerrainLayerMask = 1 << 8;

        public override string Name { get { return "SelectionGameState"; } }

        private readonly PlayerCharacter _character;
        private readonly CameraController _cameraController;

        private Material _originalMaterial = null;
        private Material _selectionMaterial = null;

        private GameObject _previousUnderMouse = null;
        

        public SelectionGameState(
            PlayerCharacter character,
            CameraController cameraController,
            Material selectionMaterial)
        {
            _character = character;
            _cameraController = cameraController;
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
                _originalMaterial = null;
                _previousUnderMouse = null;
                return;
            }

            // get object under mouse
            var underMouse = GetObjectUnderMouse();
            if (underMouse == null) { return; }
            if (_previousUnderMouse != null 
                && underMouse.name == _previousUnderMouse.name)
            {
                // same object as previous, all done here
                return;
            }

            // it's different, unswap previous item
            if (_previousUnderMouse != null)
            {
                var prevRenderer = GetRendererFromObject(_previousUnderMouse);
                prevRenderer.material = _originalMaterial;
            }

            //
            var renderer = GetRendererFromObject(underMouse);
            var currentMaterial = renderer.material;
            if (currentMaterial.name == _selectionMaterial.name)
            {
                // already applied selection material
                return;
            }

            _originalMaterial = renderer.material;
            renderer.material = _selectionMaterial;

            // save for next frame
            _previousUnderMouse = underMouse;
        }

        private GameObject GetObjectUnderMouse()
        {
            var activeCamera = CameraController.Instance.ActiveCamera;
            var underMouse = GetObjectUnderMouse(activeCamera);
            return underMouse;
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
            var go = GetObjectUnderMouse();
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
