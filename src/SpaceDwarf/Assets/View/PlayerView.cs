using UnityEngine;

namespace Assets.View
{
    public class PlayerView : MonoBehaviour
    {
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
    }
}
