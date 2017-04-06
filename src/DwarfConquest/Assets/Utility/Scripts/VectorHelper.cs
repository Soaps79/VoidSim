using UnityEngine;

namespace Assets.Utility.Scripts
{
    public static class VectorHelper
    {
        public static Vector3 GetRandom(float scalar = 1f)
        {
            return new Vector3(
                Random.value * scalar,
                Random.value * scalar,
                Random.value * scalar);
        }

        public static Vector3 GetRandom(float min, float max)
        {
            var x = Random.Range(min, max);
            var y = Random.Range(min, max);
            var z = Random.Range(min, max);
            return new Vector3(x, y, z);
        }

        public static Vector3 GetRandom01()
        {
            const float min = 0;
            const float max = 1;
            return GetRandom(min, max);
        }
    }
}