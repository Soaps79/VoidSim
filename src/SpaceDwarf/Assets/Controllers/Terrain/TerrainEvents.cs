using System.Diagnostics;
using Assets.Controllers.GUI;
using Assets.Model.Terrain;
using Assets.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers.Terrain
{
    public class TerrainEvents
    {
        public static void SetTooltip(BaseEventData arg, GameObject tileGo, TerrainTile tile, TerrainView view)
        {
            var label = string.Format("Tile {0}", tile.Type);
            var typeView = view.TileViewDictionary[tile.Type];
            var flavorText = typeView.Description;
            var thumbnail = tileGo.GetComponent<SpriteRenderer>().sprite;
            TooltipController.Instance.SetTooltip(label, flavorText, thumbnail, new Vector2(tile.X, tile.Y));
        }

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
