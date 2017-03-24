using System;
using QGame.Common;

namespace Assets.Model.Terrain
{
    /// <summary>
    /// Manages the <see cref="TerrainRegion"/>s in memory
    /// </summary>
    public class TerrainWorld : Grid<TerrainRegion>
    {
        private const int MaxWorldSize = 32;

        private static TerrainWorld _instance;
        public static TerrainWorld Instance
        {
            get { return _instance ?? (_instance = new TerrainWorld()); }
        }

        public Action<TerrainWorld, TerrainRegion> OnRegionAdded;

        private TerrainWorld()
            : base(MaxWorldSize, MaxWorldSize)
        {
        }

        public void InitializeRegions(int x = 0, int y = 0)
        {
            var centerRegion = LoadRegion(x, y);
            AddRegion(centerRegion, x, y);
        }

        public TerrainRegion LoadRegion(int x, int y)
        {
            return new TerrainRegion(x, y);
        }

        private void AddRegion(TerrainRegion region, int worldX, int worldY)
        {
            // todo: keep in bounds, recenter around 0, etc
            SetElementAt(worldX, worldY, region);

            if (OnRegionAdded != null)
                OnRegionAdded(this, region);
        }
    }
}
