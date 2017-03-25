using Assets.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.Terrain
{
    public class TerrainEvents
    {
        public static void Highlight(BaseEventData arg, GameObject tileGo, TerrainView view)
        {
            if (SelectionController.Instance.CanSelect("Terrain"))
            {
                SetMaterial(tileGo, view.HighlightMaterial);
            }

        }

        public static void Unhighlight(BaseEventData arg, GameObject tileGo, TerrainView view)
        {
            var material = (tileGo == SelectionController.Instance.SelectedObject)
                ? view.SelectedMaterial
                : view.DefaultMaterial;

            SetMaterial(tileGo, material);
        }

        private static void SetMaterial(GameObject tileGo, Material material)
        {
            var renderer = tileGo.GetComponent<SpriteRenderer>();
            renderer.material = material;
        }

    }
}
