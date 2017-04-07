using Assets.Utility.Attributes;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexFeatureManager : MonoBehaviour
    {
        public HexFeatureCollection[] UrbanCollections;
        public HexFeatureCollection[] FarmCollections;
        public HexFeatureCollection[] PlantCollections;

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

        public void AddFeature(HexCell cell, Vector3 position)
        {
            var hash = HexMetrics.SampleHashGrid(position);
            var prefab = PickPrefab(UrbanCollections, cell.UrbanLevel, hash.A, hash.D);
            var otherPrefab = PickPrefab(FarmCollections, cell.FarmLevel, hash.B, hash.D);

            float usedHash = hash.A;
            if (prefab != null)
            {
                if (otherPrefab != null && hash.B < hash.A)
                {
                    prefab = otherPrefab;
                    usedHash = hash.B;
                }
            }
            else if (otherPrefab)
            {
                prefab = otherPrefab;
                usedHash = hash.B;
            }

            otherPrefab = PickPrefab(PlantCollections, cell.PlantLevel, hash.C, hash.D);

            if (prefab != null)
            {
                if (otherPrefab != null && hash.C < usedHash)
                {
                    prefab = otherPrefab;
                }
            }
            else if (otherPrefab != null)
            {
                prefab = otherPrefab;
            }
            else
            {
                return;
            }
            

            var instance = Instantiate(prefab);

            // unity cube has origin at center, adjust
            position.y += instance.localScale.y * 0.5f;
            instance.localPosition = HexMetrics.Perturb(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
            instance.SetParent(_container, false);
        }

        private Transform PickPrefab(
            HexFeatureCollection[] collections,
            int level, float hash, float choice)
        {
            if (level > 0)
            {
                var thresholds = HexMetrics.GetFeatureThresholds(level - 1);
                for (var i = 0; i < thresholds.Length; i++)
                {
                    if (hash < thresholds[i])
                    {
                        return collections[i].Pick(choice);
                    }
                }
            }
            return null;
        }
    }
}