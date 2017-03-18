﻿using System;
using Assets.Controllers.GUI;
using Assets.Model.Terrain;
using UnityEngine;

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

            // update position relative to region parent
            tileGo.transform.position = new Vector3(x, y, 0);
            tileGo.transform.parent = region.transform;
            tileGo.layer = view.TerrainLayer;

            // create sprite component and assign texture
            var spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssignTileSprite(tile.Type, view);
            spriteRenderer.sortingLayerName = view.SortingLayerName;

            // create collider
            tileGo.AddComponent<BoxCollider>();

            // create tooltip
            var tooltip = tileGo.AddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Tile";
            tooltip.TooltipText2 = "Tile information";
            return tileGo;
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