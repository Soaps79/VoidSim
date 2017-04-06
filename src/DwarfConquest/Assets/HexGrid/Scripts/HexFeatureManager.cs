using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexFeatureManager : MonoBehaviour
    {
        public Transform FeaturePrefab;

        private Transform _container;

        public void Clear()
        {
            if (_container != null)
            {
                Destroy(_container.gameObject);
            }
            _container = new GameObject("Features Container").transform;
            _container.SetParent(transform, false);
        }

        public void Apply()
        {
            
        }

        public void AddFeature(Vector3 position)
        {
            var hash = HexMetrics.SampleHashGrid(position);
            if (hash.A >= 0.5f)
            {
                return;
            }

            var instance = Instantiate(FeaturePrefab);
            // unity cube has origin at center, adjust
            position.y += instance.localScale.y * 0.5f;
            instance.localPosition = HexMetrics.Perturb(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.B, 0f);
            instance.SetParent(_container, false);
        }
    }
}
