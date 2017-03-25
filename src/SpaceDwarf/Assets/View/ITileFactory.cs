using Assets.Model.Terrain;
using UnityEngine;

namespace Assets.View
{
    public interface ITileFactory
    {
        GameObject CreateTerrainTile(TerrainTile tile, TerrainView view, int x, int y, GameObject region);
        Sprite AssignTileSprite(TerrainType type, TerrainView view);
    }
}