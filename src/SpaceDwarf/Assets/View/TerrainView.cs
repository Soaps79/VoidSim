using System;
using System.Collections.Generic;
using Assets.Controllers;
using Assets.Model.Terrain;
using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.View
{
    public class TerrainView : OrderedEventBehavior
    {
        public int TerrainLayer = 8;
        public readonly string SortingLayerName = "Terrain";

        public Material DefaultMaterial;
        public Material HighlightMaterial;
        public Material SelectedMaterial;

        public List<TileTypeView> TileViews;

        public readonly Dictionary<TerrainType, TileTypeView> TileViewDictionary = new Dictionary<TerrainType, TileTypeView>();

        protected override void OnStart()
        {
            base.OnStart();

            // create a handy dictionary from views
            for(var i = 0; i < TileViews.Count; i++)
            {
                TileViewDictionary.Add(TileViews[i].TileType, TileViews[i]);
            }
        }

        public Sprite GetRandomSprite(TerrainType type)
        {
            var view = TileViewDictionary[type];
            var index = UnityEngine.Random.Range(0, view.Sprites.Count);
            return view.Sprites[index];
        }
    }
}
