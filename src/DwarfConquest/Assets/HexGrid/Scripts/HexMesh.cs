﻿using System;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    
    public class HexMesh : MeshBase
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            Mesh.name = "Hex Mesh";
        }

        public void Triangulate(HexCell[] cells)
        {
            Clear();

            for (var i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }

            AssignMesh();
        }

        private void Triangulate(HexCell cell)
        {
            for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Triangulate(d, cell);
            }
        }

        private void Triangulate(HexDirection direction, HexCell cell)
        {
            // solid inner region
            var center = cell.Position;
            var e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            TriangulateEdgeFan(center, e, cell.Color);

            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, e);
            }
        }

        private void TriangulateConnection(HexDirection direction, 
            HexCell cell, EdgeVertices  e1)
        {
            // blended outer sections
            var neighbor = cell.GetNeighbor(direction);
            if (neighbor == null)
            {
                // don't build blending sections on outer edges
                return;
            }

            // neighbor bridge
            var bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            var e2 = new EdgeVertices(e1.V1 + bridge, e1.V4 + bridge);
            
            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            {
                TriangulateEdgeTerraces(e1, cell, e2, neighbor);
            }
            else
            {
                TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color);
            }

            // triangle gap
            var nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                var v5 = e1.V4 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Position.y;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                    {
                        TriangulateCorner(e1.V4, cell, e2.V4, neighbor, v5, nextNeighbor);
                    }
                    else
                    {
                        TriangulateCorner(v5, nextNeighbor, e1.V4, cell, e2.V4, neighbor);
                    }
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e2.V4, neighbor, v5, nextNeighbor, e1.V4, cell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.V4, cell, e2.V4, neighbor);
                }

                //AddTriangle(v2, v4, v5);
                //AddTriangleColor(cell.Color, neighbor.Color, nextNeighbor.Color);
            }
        }

        private void TriangulateEdgeTerraces(
            EdgeVertices begin, HexCell beginCell, 
            EdgeVertices end, HexCell endCell)
        {
            var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
            var c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

            TriangulateEdgeStrip(begin, beginCell.Color, e2, c2);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var e1 = e2;
                var c1 = c2;

                e2 = EdgeVertices.TerraceLerp(begin, end, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);

                TriangulateEdgeStrip(e1, c1, e2, c2);
            }

            TriangulateEdgeStrip(e2, c2, end, endCell.Color);
        }

        private void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var leftEdgeType = bottomCell.GetEdgeType(leftCell);
            var rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.Slope)
            {
                switch (rightEdgeType)
                {
                    case HexEdgeType.Slope:
                        // SSF
                        TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                    case HexEdgeType.Flat:
                        // SFS
                        TriangulateCornerTerraces(
                            left, leftCell, right, rightCell, bottom, bottomCell);
                        break;
                    case HexEdgeType.Cliff:
                        TriangulateCornerTerracesCliff(
                            bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            string.Format("Unknown HexEdgeType: {0}", rightEdgeType), 
                            new NotImplementedException());
                }
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {
                if (leftEdgeType == HexEdgeType.Flat)
                {
                    // FSS
                    TriangulateCornerTerraces(
                        right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell);
                }
            }
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                {
                    TriangulateCornerCliffTerraces(
                        right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerTerracesCliff(
                        left, leftCell, right, rightCell, bottom, bottomCell);
                }
            }
            else
            {
                AddTriangle(bottom, left, right);
                AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
            }
        }

        private void TriangulateCornerTerraces(
            Vector3 begin, HexCell beginCell, 
            Vector3 left, HexCell leftCell, 
            Vector3 right, HexCell rightCell)
        {
            var v3 = HexMetrics.TerraceLerp(begin, left, 1);
            var v4 = HexMetrics.TerraceLerp(begin, right, 1);
            var c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
            var c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

            AddTriangle(begin, v3, v4);
            AddTriangleColor(beginCell.Color, c3, c4);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var c1 = c3;
                var c2 = c4;

                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);

                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2, c3, c4);
            }


            AddQuad(v3, v4, left, right);
            AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
        }

        private void TriangulateCornerTerracesCliff(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var b = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0) { b = -b; }

            var boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
            var boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);
            
            TriangulateBoundaryTriangle(
                begin, beginCell, left, leftCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateCornerCliffTerraces(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var b = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0) { b = -b; }

            var boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
            var boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

            TriangulateBoundaryTriangle(
                right, rightCell, begin, beginCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateBoundaryTriangle(
            Vector3 begin, HexCell beginCell, 
            Vector3 left, HexCell leftCell,
            Vector3 boundary, Color boundaryColor)
        {
            var v2 = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            var c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

            AddTriangleUnperturbed(Perturb(begin), v2, boundary);
            AddTriangleColor(beginCell.Color, c2, boundaryColor);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var v1 = v2;
                var c1 = c2;
                v2 = Perturb(HexMetrics.TerraceLerp(begin, left, i));
                c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);

                AddTriangleUnperturbed(v1, v2, boundary);
                AddTriangleColor(c1, c2, boundaryColor);
            }

            AddTriangleUnperturbed(v2, Perturb(left), boundary);
            AddTriangleColor(c2, leftCell.Color, boundaryColor);
        }

        private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
        {
            AddTriangle(center, edge.V1, edge.V2);
            AddTriangleColor(color);
            AddTriangle(center, edge.V2, edge.V3);
            AddTriangleColor(color);
            AddTriangle(center, edge.V3, edge.V4);
            AddTriangleColor(color);
        }

        private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
        {
            AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
            AddQuadColor(c1, c2);
            AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
            AddQuadColor(c1, c2);
            AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
            AddQuadColor(c1, c2);
        }

        private Vector3 Perturb(Vector3 position)
        {
            var sample = HexMetrics.SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * HexMetrics.CellPerturbStrength;
            //position.y += (sample.y * 2f - 1f) * HexMetrics.CellPerturbStrength;
            position.z += (sample.z * 2f - 1f) * HexMetrics.CellPerturbStrength;
            return position;
        }

        private void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            base.AddTriangle(v1, v2, v3);
        }

        protected override void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            base.AddTriangle(
                Perturb(v1),
                Perturb(v2),
                Perturb(v3));
        }

        protected override void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            base.AddQuad(
                Perturb(v1), 
                Perturb(v2), 
                Perturb(v3), 
                Perturb(v4));
        }
    }
}