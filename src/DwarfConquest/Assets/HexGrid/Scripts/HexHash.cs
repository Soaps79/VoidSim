using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public struct HexHash
    {
        public float A;
        public float B;

        public static HexHash Create()
        {
            HexHash hash;
            hash.A = Random.value;
            hash.B = Random.value;
            return hash;
        }
    }
}