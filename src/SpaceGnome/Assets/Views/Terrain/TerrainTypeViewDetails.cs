using System.Collections.Generic;
using Assets.Model.Terrain;
using UnityEngine;
using Zenject;

namespace Assets.Views.Terrain
{
    [CreateAssetMenu(menuName = "Terrain/Views/Tile Type")]
    public class TerrainTypeViewDetails : ScriptableObject
    {
        public TerrainType Type;

        public Material DefaultMaterial;
        public Material HighlightedMaterial;
        public Material SelectedMaterial;

        public List<Sprite> Sprites;
    }
}
