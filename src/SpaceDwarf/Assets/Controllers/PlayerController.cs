using System;
using Assets.Controllers.GUI;
using Assets.Framework;
using Assets.Model;
using Assets.View;
using UnityEngine;

namespace Assets.Controllers
{
    public class PlayerController : SingletonBehavior<PlayerController>
    {
        public PlayerView View;

        public float MoveSpeed = 5;
        

        private PlayerCharacter _character;
        private Animator _characterAnimator;
        private RectTransform _canvasTransform;

        public PlayerCharacter PlayerCharacter { get { return _character; } }
        
        void Start ()
        {
            // todo: load character profile
           _character = new PlayerCharacter(Vector2.zero);
            
            // create Player game object
            CreatePlayerView();

            // hook events
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            UpdateCharacter(delta);
            UpdateInput(delta);
        }

        private void CreatePlayerView()
        {
            var characterGo = Instantiate(View.CharacterPrefab);

            // assign parent and position
            characterGo.transform.parent = transform;
            characterGo.transform.position = _character.Position;
            characterGo.layer = 11;
            
            // grab reference to animator and canvas
            _characterAnimator = characterGo.GetComponent<Animator>();
            _canvasTransform = View.Canvas.transform as RectTransform;

            // force add collider
            characterGo.AddComponent<BoxCollider>();
            var tooltip = characterGo.GetOrAddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Character";
            tooltip.TooltipText2 = "The king baby!";

            // center camera on player
            CenterCameraOnPlayer();

            // hook player events
            _character.RegisterOnPositionChangedCallback((character, oldPosition) =>
            {
                OnCharacterPositionChanged(character, oldPosition, characterGo);
            });
        }

        private void OnCharacterPositionChanged(PlayerCharacter character, Vector2 oldPosition, GameObject characterGo)
        {
            // update game object to character position
            characterGo.transform.position = character.Position;
            
            // center camera
            CenterCameraOnPlayer();
        }

        private void CenterCameraOnPlayer()
        {
            Camera.main.transform.position = new Vector3(
                _character.Position.x, 
                _character.Position.y, 
                Camera.main.transform.position.z);
        }

        private void UpdateCharacter(float timeDelta)
        {
            UpdateCharacterPosition(timeDelta);
        }

        private void UpdateCharacterPosition(float timeDelta)
        {
            // check for input, calculate movement vector
            var horizontal = Input.GetAxis("Horizontal") * MoveSpeed * timeDelta;
            var vertical = Input.GetAxis("Vertical") * MoveSpeed * timeDelta;
            var movement = new Vector2(horizontal, vertical);

            // update animation
            _characterAnimator.SetFloat("MoveX", horizontal);
            _characterAnimator.SetFloat("MoveY", vertical);

            //Debug.Log(string.Format("MoveY: ({0})", vertical));

            // update player character
            _character.Move(movement);
        }

        private void UpdateInput(float timeDelta)
        {
            // scroll wheel to zoom
            HandleMouseZoom(timeDelta);
            HandleMenuInput(timeDelta);
        }

        private void HandleMenuInput(float timeDelta)
        {
            // check for inventory
            if (Input.GetButtonDown("Inventory"))
            {
                TogglePanel(View.InventoryMenu);
            }

            // check for build menu
            if (Input.GetButtonDown("BuildPalette"))
            {
                TogglePanel(View.BuildPalette);
            }
        }

        private void TogglePanel(GameObject panel)
        {
            if (!panel.activeSelf)
            {
                ClampPanelToScreen(panel);
            }
            
            // toggle panel
            panel.SetActive(!panel.activeSelf);
        }

        private void ClampPanelToScreen(GameObject panel)
        {
            // spawn under mouse but keep on screen
            var pointer = Input.mousePosition;

            var clampedPosition = ClampPanelToScreen(panel.transform as RectTransform, _canvasTransform, pointer);

            // transform directly in screen space
            panel.transform.position = clampedPosition;
        }

        private static Vector2 ClampPanelToScreen(RectTransform panel, RectTransform canvas, Vector3 pointer)
        {
            // get corners of canvas
            var canvasCorners = new Vector3[4];
            canvas.GetWorldCorners(canvasCorners);
            

            var clampedX = Mathf.Clamp(pointer.x, canvasCorners[0].x, canvasCorners[2].x - panel.rect.width);
            var clampedY = Mathf.Clamp(pointer.y, canvasCorners[0].y + panel.rect.height, canvasCorners[2].y);

            var clampedPosition = new Vector2(clampedX, clampedY);
            return clampedPosition;
        }

        private void HandleMouseZoom(float timeDelta)
        {
            var magnitude = timeDelta * View.ZoomSpeed;
            if (!View.SmoothZoom)
                magnitude = 1;

            // backward
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + magnitude, View.MaxOrthoSize);
            }

            // forward
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - magnitude, View.MinOrthoSize);
            }
        }

        
    }
}
