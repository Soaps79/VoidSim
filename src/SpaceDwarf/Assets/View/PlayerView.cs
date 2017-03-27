using Assets.Controllers.GUI;
using Assets.Controllers.Player;
using Assets.Framework;
using Assets.Model;
using Assets.Model.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.View
{
    [RequireComponent(typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {
        private const int Layer = 11;
        // character object and components
        public GameObject CharacterPrefab;

        public float MoveSpeed = 5;

        // gui
        public Canvas Canvas;
        public GameObject InventoryMenu;
        public GameObject BuildPalette;
        
        // controller access
        public Animator Animator { get; private set; }
        public RectTransform CanvasTransform { get; private set; }

        public void Initialize(PlayerCharacter character)
        {
            // offset position from world space to view space (-0.5*RegionSize)
            var offset = -0.5f * TerrainRegion.RegionSize;
            var viewPosition = new Vector2(character.Position.x + offset, character.Position.y + offset);
            // assign parent and position
            CharacterPrefab.transform.position = viewPosition;
            CharacterPrefab.layer = Layer;

            // grab reference to animator and canvas
            Animator = CharacterPrefab.GetComponent<Animator>();
            CanvasTransform = Canvas.transform as RectTransform;

            // add event system
            var eventTrigger = CharacterPrefab.GetOrAddComponent<EventTrigger>();
            var enterTrigger = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            var onEnter = new EventTrigger.TriggerEvent();
            onEnter.AddListener((data) => PlayerEvents.SetTooltip(this, character));
            enterTrigger.callback = onEnter;
            eventTrigger.triggers.Add(enterTrigger);

        }
    }
}
