using System.Collections.Generic;
using Assets.Model;
using Assets.Model.Terrain;
using Assets.View;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Controllers.Player
{
    public class PlayerController : SingletonBehavior<PlayerController>
    {
        public PlayerView View;
        
        [Inject] public PlayerCharacter PlayerCharacter { get; set; }

        public List<PlayerControl> Controls;

        protected override void OnStart ()
        {
            // create Player game object
            View.Initialize(PlayerCharacter);

            // hook events
            PlayerCharacter.RegisterOnPositionChangedCallback((character, oldPosition) =>
            {
                OnCharacterPositionChanged(character, oldPosition, View.CharacterPrefab);
            });
        }

        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            UpdateCharacter(delta);
            HandleMenuInput();
        }

        private void OnCharacterPositionChanged(PlayerCharacter character, Vector2 oldPosition, GameObject characterGo)
        {
            // update game object to character position
            // offset to from world space to view space (-0.5f*RegionSize)
            var offset = -0.5f * TerrainRegion.RegionSize;
            var viewPosition = new Vector2(character.Position.x + offset, character.Position.y + offset);
            characterGo.transform.position = viewPosition;
        }

        private void UpdateCharacter(float timeDelta)
        {
            for (var i = 0; i < Controls.Count; i++)
            {
                Controls[i].UpdateCharacter(PlayerCharacter, View, timeDelta);
            }
        }

        private void HandleMenuInput()
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
                ClampPanelToScreen(panel, View.CanvasTransform);
            }
            
            // toggle panel
            panel.SetActive(!panel.activeSelf);
        }

        private static void ClampPanelToScreen(GameObject panel, RectTransform canvasTransform)
        {
            // spawn under mouse but keep on screen
            var pointer = Input.mousePosition;

            var clampedPosition = ClampPanelToScreen(panel.transform as RectTransform, canvasTransform, pointer);

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
    }
}
