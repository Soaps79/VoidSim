using System;
using Assets.Controllers.GUI;
using Assets.Framework;
using Assets.Model.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.View
{
    public interface ITileFactory
    {
        GameObject CreateTerrainTile(TerrainTile tile, TerrainView view, int x, int y, GameObject region);
        Sprite AssignTileSprite(TerrainType type, TerrainView view);
    }

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
            
            tileGo.name = tile.ToString();
            tileGo.transform.parent = region.transform;
            tileGo.transform.localPosition = new Vector3(x, y, 1);
            tileGo.layer = view.TerrainLayer;

            // create sprite component and assign texture
            var spriteRenderer = tileGo.GetOrAddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssignTileSprite(tile.Type, view);
            spriteRenderer.sortingLayerName = view.SortingLayerName;

            // create collider
            //tileGo.AddComponent<BoxCollider2D>();

            // create tooltip
            var tooltip = tileGo.GetOrAddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Tile";
            tooltip.TooltipText2 = "Tile information";

            // add event trigger
            
            AddEventTriggers(view, tileGo);
            return tileGo;
        }

        private void AddEventTriggers(TerrainView view, GameObject tileGo)
        {
            var trigger = tileGo.AddComponent<EventTrigger>();

            var enter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            var onEnter = new EventTrigger.TriggerEvent();
            onEnter.AddListener((data) => Highlight(data, tileGo, view));
            enter.callback = onEnter;
            trigger.triggers.Add(enter);

            var exit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            var onExit = new EventTrigger.TriggerEvent();
            onExit.AddListener((data) => Unhighlight(data, tileGo, view));
            exit.callback = onExit;
            trigger.triggers.Add(exit);
        }

        private void Unhighlight(BaseEventData arg, GameObject tileGo, TerrainView view)
        {
            SetMaterial(tileGo, view.DefaultMaterial);
        }

        private void SetMaterial(GameObject tileGo, Material material)
        {
            var renderer = tileGo.GetComponent<SpriteRenderer>();
            renderer.material = material;
        }


        public void Highlight(BaseEventData arg, GameObject tileGo, TerrainView view)
        {
            //Debug.Log(string.Format("Swap from Tile hit! Object: {0}", tileGo.name));
            SetMaterial(tileGo, view.HighlightMaterial);
        }

        public Sprite AssignTileSprite(TerrainType type, TerrainView view)
        {
            return view.GetRandomSprite(type);
        }

    }
}
