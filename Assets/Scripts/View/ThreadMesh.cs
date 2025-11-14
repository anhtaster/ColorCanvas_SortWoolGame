using UnityEngine;
using System.Collections.Generic;

public class ThreadMesh3D : MonoBehaviour
{
    public int radialSegments = 12;
    public float radius = 0.03f;

    private Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer renderer;

    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();

        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();

        ResetMesh();
    }

    public void SetMaterial(Material material)
    {
        if (renderer != null)
            renderer.material = material;
    }

    public void ResetMesh()
    {
        mesh = new Mesh();
        mesh.name = "ThreadMesh";
        filter.sharedMesh = mesh;
    }

    public void GenerateTube(List<Vector3> points)
    {
        if (points == null || points.Count < 2)
            return;

        int n = points.Count;
        int ringVerts = radialSegments + 1;

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector3> normals = new();
        List<Vector2> uvs = new();

        // Generate vertices
        for (int i = 0; i < n; i++)
        {
            Vector3 center = points[i];

            Vector3 forward = (i < n - 1)
                ? (points[i + 1] - points[i]).normalized
                : (points[i] - points[i - 1]).normalized;

            Vector3 side = Vector3.Cross(forward, Vector3.up);

            if (side.sqrMagnitude < 0.001f)
                side = Vector3.Cross(forward, Vector3.right);

            Vector3 up = Vector3.Cross(side, forward).normalized;

            Quaternion rot = Quaternion.LookRotation(forward, up);

            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = (j / (float)radialSegments) * Mathf.PI * 2f;
                Vector3 local = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);

                vertices.Add(center + rot * local);
                normals.Add((rot * local).normalized);
                uvs.Add(new Vector2(j / (float)radialSegments, i / (float)n));
            }
        }

        // Generate triangles
        for (int i = 0; i < n - 1; i++)
        {
            int start = i * ringVerts;
            int next = (i + 1) * ringVerts;

            for (int j = 0; j < radialSegments; j++)
            {
                int a = start + j;
                int b = start + j + 1;
                int c = next + j;
                int d = next + j + 1;

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);

                triangles.Add(b);
                triangles.Add(c);
                triangles.Add(d);
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
    }
}
