using System.Collections.Generic;
using Assets.Model.Terrain;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Tile View")]
public class TileTypeView : ScriptableObject
{
    public TerrainType TileType;
    public List<Sprite> Sprites;

    public Material DefaultMaterial;
    public Material HighlightMaterial;
    public Material SelectedMaterial;
}