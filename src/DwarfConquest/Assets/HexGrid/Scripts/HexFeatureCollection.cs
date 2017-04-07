using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    [System.Serializable]
    public struct HexFeatureCollection
    {
        public Transform[] Prefabs;

        public Transform Pick(float choice)
        {
            return Prefabs[(int)(choice * Prefabs.Length)];
        }
    }
}