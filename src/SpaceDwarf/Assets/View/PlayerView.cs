using Assets.Controllers.GUI;
using Assets.Framework;
using Assets.Model;
using UnityEngine;

namespace Assets.View
{
    [RequireComponent(typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {
        private const int Layer = 11;
        // character object and components
        public GameObject CharacterPrefab;

        // gui
        public Canvas Canvas;
        public GameObject InventoryMenu;
        public GameObject BuildPalette;

        // camera
        public bool SmoothZoom = false;
        public float ZoomSpeed = 40;
        public float MaxOrthoSize = 16;
        public float MinOrthoSize = 2;

        // controller access
        public Animator Animator { get; private set; }
        public RectTransform CanvasTransform { get; private set; }

        public void Initialize(PlayerCharacter character)
        {
            // assign parent and position
            CharacterPrefab.transform.position = character.Position;
            CharacterPrefab.layer = Layer;

            // grab reference to animator and canvas
            Animator = CharacterPrefab.GetComponent<Animator>();
            CanvasTransform = Canvas.transform as RectTransform;

            // force add collider, this works but revisit
            CharacterPrefab.AddComponent<BoxCollider>();
            var tooltip = CharacterPrefab.GetOrAddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Character";
            tooltip.TooltipText2 = "The king baby!";
        }
    }
}
