using UnityEngine;

namespace Assets.Utility.Scripts
{
    public class HashGrid
    {
        private Vector3[] _grid;

        public int GridSize { get; private set; }
        public float GridScale { get; private set; }

        public HashGrid(int size = 256, float scale = 1f)
        {
            GridSize = size;
            GridScale = scale;
        }

        public void Initialize(int seed)
        {
            _grid = new Vector3[GridSize * GridSize];
            var currentState = Random.state;
            Random.InitState(seed);
            for (var i = 0; i < _grid.Length; i++)
            {
                _grid[i] = VectorHelper.GetRandom();
            }
            Random.state = currentState;
        }

        public Vector3 SampleGrid(Vector3 position)
        {
            var x = (int)(position.x * GridScale) % GridSize;
            if (x < 0) { x += GridSize; }

            var z = (int)(position.z * GridScale) % GridSize;
            if (z < 0) { z += GridSize; }

            return _grid[x + z * GridSize];
        }
    }
}