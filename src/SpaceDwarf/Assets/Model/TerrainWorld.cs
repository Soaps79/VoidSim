
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Assets.Model
{
    public class TerrainWorld
    {
        private const int MaxRegionWidth = 3;
        private const int MaxRegionHeight = 3;
        

        private TerrainRegion[,] _regions = new TerrainRegion[MaxRegionWidth, MaxRegionHeight];


        private Action<TerrainWorld, TerrainRegion> _onRegionAdded;

        public TerrainWorld()
        {
            
        }

        public void InitializeRegions(int x = 0, int y = 0)
        {
            var centerRegion = new TerrainRegion(x, y);
            AddRegion(centerRegion, 1, 1);
            
            // todo: create adjacent regions
        }

        private void AddRegion(TerrainRegion region, int regionViewX, int regionViewY)
        {
            if (regionViewX > MaxRegionWidth || regionViewX < 0)
            {
                throw new IndexOutOfRangeException("regionViewX TerrainWorld index out of bounds");
            }
            if (regionViewY > MaxRegionHeight || regionViewY < 0)
            {
                throw new IndexOutOfRangeException("regionViewY TerrainWorld index out of bounds");
            }

            _regions[regionViewX, regionViewY] = region;
            
            // fire events
            if (_onRegionAdded != null)
                _onRegionAdded(this, region);
        }

        private TerrainRegion CreateRegion(int x, int y)
        {
           return new TerrainRegion(x, y);
        }

        public void RegisterOnRegionAddedCallback(Action<TerrainWorld, TerrainRegion> callback)
        {
            _onRegionAdded += callback;
        }
    }
}
