using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public struct HexHash
    {
        public float A;
        public float B;
        public float C;
        public float D;
        public float E;

        public static HexHash Create()
        {
            HexHash hash;
            hash.A = Random.value * 0.999f;
            hash.B = Random.value * 0.999f;
            hash.C = Random.value * 0.999f;
            hash.D = Random.value * 0.999f;
            hash.E = Random.value * 0.999f;
            return hash;
        }
    }
}