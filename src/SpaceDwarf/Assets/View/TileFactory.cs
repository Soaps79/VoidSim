﻿using System;
using Assets.Controllers.GUI;
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
            var tileGo = new GameObject(tile.ToString());

            // update position relative to region parent (not accurately working!!!)
            tileGo.transform.parent = region.transform;
            tileGo.transform.localPosition = new Vector3(x, y, 1);
            tileGo.layer = view.TerrainLayer;

            // create sprite component and assign texture
            var spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssignTileSprite(tile.Type, view);
            spriteRenderer.sortingLayerName = view.SortingLayerName;

            // create collider
            tileGo.AddComponent<BoxCollider2D>();

            // create tooltip
            var tooltip = tileGo.AddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Tile";
            tooltip.TooltipText2 = "Tile information";

            // add event trigger
            var trigger = tileGo.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            var callback = new EventTrigger.TriggerEvent();
            callback.AddListener((data) => SwapMaterial(data, tileGo));
            entry.callback = callback;
            trigger.triggers.Add(entry);
            return tileGo;
        }

        public void SwapMaterial(BaseEventData arg, GameObject tileGo)
        {
            Debug.Log(string.Format("Swap from Tile hit! Object: {0}", tileGo.name));
        }

        public Sprite AssignTileSprite(TerrainType type, TerrainView view)
        {
            switch (type)
            {
                case TerrainType.GreenGrass:
                    return view.GetGreenGrassSprite();
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

    }
}
