using Assets.Utility.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Assets.HexGrid.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        public bool UseCollider;
        public bool UseColors;
        public bool UseUVCoordinates;
        public bool UseUV2Coordinates;

        private Mesh _mesh;
        [NonSerialized] private List<Vector3> _vertices;
        [NonSerialized] private List<int> _triangles;
        [NonSerialized] private List<Color> _colors;
        [NonSerialized] private List<Vector2> _uvs;
        [NonSerialized] private List<Vector2> _uv2s;

        private MeshCollider _meshCollider;

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Hex Mesh";

            if (UseCollider)
            {
                _meshCollider = gameObject.AddComponent<MeshCollider>();
            }
        }

        public void Clear()
        {
            _mesh.Clear();
            _vertices = ListPool<Vector3>.Get();
            _triangles = ListPool<int>.Get();

            if (UseColors)
            {
                _colors = ListPool<Color>.Get();
            }

            if (UseUVCoordinates)
            {
                _uvs = ListPool<Vector2>.Get();
            }

            if (UseUV2Coordinates)
            {
                _uv2s = ListPool<Vector2>.Get();
            }
        }

        public void Apply()
        {
            _mesh.SetVertices(_vertices);
            ListPool<Vector3>.Add(_vertices);

            _mesh.SetTriangles(_triangles, 0);
            ListPool<int>.Add(_triangles);

            if (UseColors)
            {
                _mesh.SetColors(_colors);
                ListPool<Color>.Add(_colors);
            }

            if (UseUVCoordinates)
            {
                _mesh.SetUVs(0, _uvs);
                ListPool<Vector2>.Add(_uvs);
            }

            if (UseUV2Coordinates)
            {
                _mesh.SetUVs(1, _uv2s);
                ListPool<Vector2>.Add(_uv2s);
            }

            _mesh.RecalculateNormals();
            if (UseCollider)
            {
                _meshCollider.sharedMesh = _mesh;
            }
        }

        #region Add Triangles

        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);

            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            AddTriangleUnperturbed(HexMetrics.Perturb(v1), HexMetrics.Perturb(v2), HexMetrics.Perturb(v3));
        }

        public void AddTriangleColor(Color color)
        {
            AddTriangleColor(color, color, color);
        }

        public void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
        }

        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            _uvs.Add(uv1);
            _uvs.Add(uv2);
            _uvs.Add(uv3);
        }

        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            _uv2s.Add(uv1);
            _uv2s.Add(uv2);
            _uv2s.Add(uv3);
        }

        #endregion Add Triangles

        #region Add Quads

        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            AddQuadUnperturbed(HexMetrics.Perturb(v1), HexMetrics.Perturb(v2), HexMetrics.Perturb(v3), HexMetrics.Perturb(v4));
        }

        public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);

            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);

            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
        }

        public void AddQuadColor(Color color)
        {
            AddQuadColor(color, color, color, color);
        }

        public void AddQuadColor(Color c1, Color c2)
        {
            AddQuadColor(c1, c1, c2, c2);
        }

        public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
            _colors.Add(c4);
        }

        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            _uvs.Add(uv1);
            _uvs.Add(uv2);
            _uvs.Add(uv3);
            _uvs.Add(uv4);
        }

        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            _uvs.Add(new Vector2(uMin, vMin));
            _uvs.Add(new Vector2(uMax, vMin));
            _uvs.Add(new Vector2(uMin, vMax));
            _uvs.Add(new Vector2(uMax, vMax));
        }

        public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            _uv2s.Add(uv1);
            _uv2s.Add(uv2);
            _uv2s.Add(uv3);
            _uv2s.Add(uv4);
        }

        public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
        {
            _uv2s.Add(new Vector2(uMin, vMin));
            _uv2s.Add(new Vector2(uMax, vMin));
            _uv2s.Add(new Vector2(uMin, vMax));
            _uv2s.Add(new Vector2(uMax, vMax));
        }

        #endregion Add Quads
    }
}