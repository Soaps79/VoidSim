using System.Collections.Generic;
using UnityEngine;

namespace Assets.HexGrid.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class MeshBase : MonoBehaviour
    {
        protected Mesh Mesh;
        protected List<Vector3> Vertices;
        protected List<int> Triangles;
        
        protected MeshCollider MeshCollider;
        
        protected List<Color> Colors;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            GetComponent<MeshFilter>().mesh = Mesh = new Mesh();
            MeshCollider = gameObject.AddComponent<MeshCollider>();

            Vertices = new List<Vector3>();
            Triangles = new List<int>();
            Colors = new List<Color>();
        }

        protected void Clear()
        {
            Mesh.Clear();
            Vertices.Clear();
            Triangles.Clear();
            Colors.Clear();
        }

        protected void AssignMesh()
        {
            Mesh.vertices = Vertices.ToArray();
            Mesh.triangles = Triangles.ToArray();
            Mesh.colors = Colors.ToArray();
            Mesh.RecalculateNormals();
            MeshCollider.sharedMesh = Mesh;
        }

        protected void AddTriangleColor(Color color)
        {
            AddTriangleColor(color, color, color);
        }

        protected void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c3);
        }

        protected virtual void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = Vertices.Count;
            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);

            
            Triangles.Add(vertexIndex);
            Triangles.Add(vertexIndex + 1);
            Triangles.Add(vertexIndex + 2);
        }

        protected virtual void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = Vertices.Count;
            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);
            Vertices.Add(v4);

            Triangles.Add(vertexIndex);
            Triangles.Add(vertexIndex + 2);
            Triangles.Add(vertexIndex + 1);

            Triangles.Add(vertexIndex + 1);
            Triangles.Add(vertexIndex + 2);
            Triangles.Add(vertexIndex + 3);
        }

        protected void AddQuadColor(Color c1, Color c2)
        {
            AddQuadColor(c1, c1, c2, c2);
        }

        protected void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c3);
            Colors.Add(c4);
        }
    }
}
