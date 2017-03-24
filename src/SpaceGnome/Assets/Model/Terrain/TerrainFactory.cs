using System.Collections.Generic;
using Assets.Controllers;
using Assets.Views.Terrain;
using UnityEngine;

namespace Assets.Model.Terrain
{
    public class TerrainFactory
    {
        public GameObject CreateTile(TerrainTile tile, List<TerrainTypeViewDetails> views, int x, int y, GameObject regionGo)
        {
            var tileGo = new GameObject(tile.ToString());
            tileGo.transform.parent = regionGo.transform;
            tileGo.transform.localPosition = new Vector3(x, y, 1);

            // create sprite component and assign texture
            var spriteRenderer = tileGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssignTileSprite(tile.Type, views);
            
            // create collider
            tileGo.AddComponent<BoxCollider2D>();

            return tileGo;
        }

        public Sprite AssignTileSprite(TerrainType type, List<TerrainTypeViewDetails> views)
        {
            var view = TerrainController.Instance.ViewDictionary[type];
            var index = Random.Range(0, view.Sprites.Count);
            return view.Sprites[index];
        }

        public GameObject CreateRegion(string name, Transform root)
        {
            var regionGo = new GameObject(name);
            regionGo.transform.parent = root;
            regionGo.transform.localPosition = new Vector3(0, 0, 0);

            return regionGo;
        }
    }
}
