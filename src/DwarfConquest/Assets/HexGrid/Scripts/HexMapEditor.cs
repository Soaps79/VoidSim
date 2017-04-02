using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.HexGrid.Scripts
{
    public class HexMapEditor : MonoBehaviour
    {
        public Color[] Colors;
        public HexGrid HexGrid;

        private Color _activeColor;

        private void Awake()
        {
            SelectColor(0);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexGrid.ColorCell(hit.point, _activeColor);
            }
        }

        public void SelectColor(int i)
        {
            _activeColor = Colors[i];
        }
    }
}