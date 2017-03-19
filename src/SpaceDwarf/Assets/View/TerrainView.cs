using System;
using UnityEngine;

namespace Assets.View
{
    public class TerrainView : MonoBehaviour
    {
        public int TerrainLayer = 8;
        public readonly string SortingLayerName = "Terrain";

        public Sprite GreenGrassTile0;
        public Sprite GreenGrassTile1;
        public Sprite GreenGrassTile2;
        public Sprite GreenGrassTile3;

        public Sprite GetGreenGrassSprite()
        {
            var type = UnityEngine.Random.Range(0, 4);
            switch (type)
            {
                case 0:
                    return GreenGrassTile0;
                case 1:
                    return GreenGrassTile1;
                case 2:
                    return GreenGrassTile2;
                case 3:
                    return GreenGrassTile3;
                default:
                    Debug.LogError(new IndexOutOfRangeException("Invalid GreenGrassTile index."));
                    return GreenGrassTile0;
            }
        }
    }
}
