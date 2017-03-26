using Assets.Controllers;
using Assets.Controllers.GUI;
using Assets.Controllers.Terrain;
using Assets.Framework;
using Assets.Model.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.View
{
    public class TileFactory  : ITileFactory
    {
        public GameObject CreateTerrainTile(TerrainTile tile, TerrainView view, int x, int y, GameObject region)
        {
            var tileGo = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TerrainTile")) as GameObject;
            if (tileGo == null)
            {
                Debug.LogError("Could not load tile.");
                return null;
            }
            
            SetWorldDetails(tile, view, x, y, region, tileGo);
            SetSpriteComponents(tile, view, tileGo);
            SetEventTriggers(view, tileGo, tile);
            
            // create tooltip
            //var tooltip = tileGo.GetOrAddComponent<TooltipBehavior>();
            //tooltip.TooltipText1 = string.Format("{0} Tile", tile.Type);
            //tooltip.TooltipText2 = string.Format("{0}", tileGo.name);
            
            return tileGo;
        }

        private void SetSpriteComponents(TerrainTile tile, TerrainView view, GameObject tileGo)
        {
            // create sprite component and assign texture
            var spriteRenderer = tileGo.GetOrAddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssignTileSprite(tile.Type, view);
            spriteRenderer.sortingLayerName = view.SortingLayerName;
        }

        private static void SetWorldDetails(TerrainTile tile, TerrainView view, int x, int y, GameObject region,
            GameObject tileGo)
        {
            tileGo.name = tile.ToString();
            tileGo.transform.parent = region.transform;
            tileGo.transform.localPosition = new Vector3(x, y, 1);
            tileGo.layer = view.TerrainLayer;
        }

        public Sprite AssignTileSprite(TerrainType type, TerrainView view)
        {
            return view.GetRandomSprite(type);
        }

        private void SetEventTriggers(TerrainView view, GameObject tileGo, TerrainTile tile)
        {
            var trigger = tileGo.GetOrAddComponent<EventTrigger>();

            var enter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            var onEnter = new EventTrigger.TriggerEvent();
            onEnter.AddListener((data) => TerrainEvents.Highlight(data, tileGo, view));
            onEnter.AddListener((data) => TerrainEvents.SetTooltip(data, tileGo, tile, view));
            enter.callback = onEnter;
            trigger.triggers.Add(enter);

            var exit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            var onExit = new EventTrigger.TriggerEvent();
            onExit.AddListener((data) => TerrainEvents.Unhighlight(data, tileGo, view));
            exit.callback = onExit;
            trigger.triggers.Add(exit);
        }
    }
}
