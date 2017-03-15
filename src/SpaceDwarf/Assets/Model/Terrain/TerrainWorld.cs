using System;
using Assets.Framework;

namespace Assets.Model.Terrain
{
    /// <summary>
    /// Grid of <see cref="TerrainRegion"/> for memory management. Maintains a 3x3 grid
    /// with the center <see cref="TerrainRegion"/> as the focus.
    /// </summary>
    public class TerrainWorld : Grid<TerrainRegion>
    {
        private const int MaxRegionWidth = 3;
        private const int MaxRegionHeight = 3;
        
        private Action<TerrainWorld, TerrainRegion> _onRegionAdded;

        public TerrainWorld()
            : base(MaxRegionWidth, MaxRegionHeight)
        {
        }
        
        public void InitializeRegions(int x = 0, int y = 0)
        {
            var centerRegion = new TerrainRegion(x, y);

            //todo: calculate center index
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

            SetObjectAt(regionViewX, regionViewY, region);
            
            // fire events
            if (_onRegionAdded != null)
                _onRegionAdded(this, region);
        }

        public void RegisterOnRegionAddedCallback(Action<TerrainWorld, TerrainRegion> callback)
        {
            _onRegionAdded += callback;
        }
    }
}
