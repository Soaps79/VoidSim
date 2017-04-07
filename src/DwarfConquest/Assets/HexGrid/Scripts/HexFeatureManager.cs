using System.Collections.Generic;
using Assets.Utility.Attributes;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    public class HexFeatureManager : MonoBehaviour
    {
        public HexFeatureCollection[] UrbanCollections;
        public HexFeatureCollection[] FarmCollections;
        public HexFeatureCollection[] PlantCollections;

        [RequireReference]
        public HexMesh Walls;

        [RequireReference]
        public Transform WallTower;

        public Transform[] Special;

        private Transform _container;

        public void Clear()
        {
            if (_container != null)
            {
                Destroy(_container.gameObject);
            }
            _container = new GameObject("Features Container").transform;
            _container.SetParent(transform, false);

            Walls.Clear();
        }

        public void Apply()
        {
            Walls.Apply();
        }

        public void AddWall(
            EdgeVertices near, HexCell nearCell,
            EdgeVertices far, HexCell farCell,
            bool hasRiver, bool hasGap)
        {
            if (nearCell.IsWalled != farCell.IsWalled
                && !nearCell.IsUnderwater && !farCell.IsUnderwater
                && nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff)
            {
                AddWallSegment(near.V1, far.V1, near.V2, far.V2);
                if (hasRiver || hasGap)
                {
                    // leave a gap
                    AddWallCap(near.V2, far.V2);
                    AddWallCap(far.V4, near.V4);
                }
                else
                {
                    AddWallSegment(near.V2, far.V2, near.V3, far.V3);
                    AddWallSegment(near.V3, far.V3, near.V4, far.V4);
                }
                AddWallSegment(near.V4, far.V4, near.V5, far.V5);
            }
        }

        public void AddWall(
            Vector3 c1, HexCell cell1,
            Vector3 c2, HexCell cell2,
            Vector3 c3, HexCell cell3)
        {
            if (cell1.IsWalled)
            {
                if (cell2.IsWalled)
                {
                    if (!cell3.IsWalled)
                    {
                        AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
                    }
                }
                else if (cell3.IsWalled)
                {
                    AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
                }
                else
                {
                    AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
                }
            }
            else if (cell2.IsWalled)
            {
                if (cell3.IsWalled)
                {
                    AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
                }
                else
                {
                    AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
                }
            }
            else if (cell3.IsWalled)
            {
                AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
            }
        }

        private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
        {
            near = HexMetrics.Perturb(near);
            far = HexMetrics.Perturb(far);
            point = HexMetrics.Perturb(point);

            var center = HexMetrics.WallLerp(near, far);
            var thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v1, v2, v3, v4;
            var pointTop = point;
            point.y = center.y;

            v1 = v3 = center - thickness;
            v2 = v4 = center + thickness;
            v3.y = v4.y = pointTop.y = center.y + HexMetrics.WallHeight;

            Walls.AddQuadUnperturbed(v1, point, v3, pointTop);
            Walls.AddQuadUnperturbed(point, v2, pointTop, v4);
            Walls.AddTriangleUnperturbed(pointTop, v3, v4);
        }

        private void AddWallCap(Vector3 near, Vector3 far)
        {
            near = HexMetrics.Perturb(near);
            far = HexMetrics.Perturb(far);

            var center = HexMetrics.WallLerp(near, far);
            var thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v1, v2, v3, v4;

            v1 = v3 = center - thickness;
            v2 = v4 = center + thickness;
            v3.y = v4.y = center.y + HexMetrics.WallHeight;

            Walls.AddQuadUnperturbed(v1, v2, v3, v4);
        }

        private void AddWallSegment(
            Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight,
            bool addTower = false)
        {
            nearLeft = HexMetrics.Perturb(nearLeft);
            farLeft = HexMetrics.Perturb(farLeft);
            nearRight = HexMetrics.Perturb(nearRight);
            farRight = HexMetrics.Perturb(farRight);

            var left = HexMetrics.WallLerp(nearLeft, farLeft);
            var right = HexMetrics.WallLerp(nearRight, farRight);

            var leftThicknessOffset = HexMetrics.WallThicknessOffset(nearLeft, farLeft);
            var rightThicknessOffset = HexMetrics.WallThicknessOffset(nearRight, farRight);

            var leftTop = left.y + HexMetrics.WallHeight;
            var rightTop = right.y + HexMetrics.WallHeight;

            Vector3 v1, v2, v3, v4;

            // frontside
            v1 = v3 = left - leftThicknessOffset;
            v2 = v4 = right - rightThicknessOffset;
            v3.y = leftTop;
            v4.y = rightTop;
            Walls.AddQuadUnperturbed(v1, v2, v3, v4);

            var t1 = v3;
            var t2 = v4;

            // backside
            v1 = v3 = left + leftThicknessOffset;
            v2 = v4 = right + rightThicknessOffset;
            v3.y = leftTop;
            v4.y = rightTop;
            Walls.AddQuadUnperturbed(v2, v1, v4, v3);

            // top segment
            Walls.AddQuadUnperturbed(t1, t2, v3, v4);

            if (addTower)
            {
                var towerInstance = Instantiate(WallTower);
                towerInstance.transform.localPosition = (left + right) * 0.5f;
                var rightDirection = right - left;
                rightDirection.y = 0;
                towerInstance.transform.right = rightDirection;
                towerInstance.SetParent(_container, false);
            }
        }

        private void AddWallSegment(
            Vector3 pivot, HexCell pivotCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            if (pivotCell.IsUnderwater)
            {
                return;
            }

            var hasLeftWall = !leftCell.IsUnderwater
                && pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
            var hasRightWall = !rightCell.IsUnderwater
                               && pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

            if (hasLeftWall)
            {
                if (hasRightWall)
                {
                    AddConnectingWall(pivot, left, leftCell, right, rightCell);
                }
                else
                {
                    AddOnlyLeftWall(pivot, left, leftCell, right, rightCell);
                }
            }
            else if (hasRightWall)
            {
                AddOnlyRightWall(pivot, left, leftCell, right, rightCell);
            }
        }

        private void AddOnlyLeftWall(Vector3 pivot, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            if (leftCell.Elevation < rightCell.Elevation)
            {
                AddWallWedge(pivot, left, right);
            }
            else
            {
                AddWallCap(pivot, left);
            }
        }

        private void AddConnectingWall(Vector3 pivot, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            var hasTower = false;
            if (leftCell.Elevation == rightCell.Elevation)
            {
                var hash = HexMetrics.SampleHashGrid(
                    (pivot + left + right) * (1f / 3f));
                hasTower = hash.E < HexMetrics.WallTowerThreshold;
            }
            AddWallSegment(pivot, left, pivot, right, hasTower);
        }

        private void AddOnlyRightWall(Vector3 pivot, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            if (rightCell.Elevation < leftCell.Elevation)
            {
                AddWallWedge(right, pivot, left);
            }
            else
            {
                AddWallCap(right, pivot);
            }
        }

        public void AddFeature(HexCell cell, Vector3 position)
        {
            if (cell.IsSpecial) { return; }

            var hash = HexMetrics.SampleHashGrid(position);
            var prefab = SelectPrefabForCell(cell, position, hash);

            if(prefab == null)
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

        private Transform SelectPrefabForCell(HexCell cell, Vector3 position, HexHash hash)
        {

            var prefabs = new Dictionary<float, Transform>
            {
                {hash.A, PickPrefab(UrbanCollections, cell.UrbanLevel, hash.A, hash.D)},
                {hash.B, PickPrefab(FarmCollections, cell.FarmLevel, hash.B, hash.D)},
                {hash.C, PickPrefab(PlantCollections, cell.PlantLevel, hash.C, hash.D)}
            };

            var usedHash = float.MinValue;
            Transform prefab = null;
            foreach (var pair in prefabs)
            {
                if (pair.Value == null
                    || pair.Key < usedHash)
                {
                    continue;
                }
                
                usedHash = pair.Key;
                prefab = pair.Value;
            }
            return prefab;
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

        public void AddSpecialFeature(HexCell cell, Vector3 position)
        {
            if (cell.SpecialIndex <= 0) return;

            var instance = Instantiate(Special[cell.SpecialIndex - 1]);
            instance.localPosition = HexMetrics.Perturb(position);
            var hash = HexMetrics.SampleHashGrid(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
            instance.SetParent(_container, false);
        }
    }
}