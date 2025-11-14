using UnityEngine;
using System.Collections.Generic;

public class ThreadMesh3D : MonoBehaviour
{
    [SerializeField] private int radialSegments = 12;
    [SerializeField] public float radius = 0.025f;
    [SerializeField] private Material material;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    public void SetMaterial(Material mat)
    {
        material = mat;
        if (meshRenderer != null)
            meshRenderer.material = material;
    }

    public void GenerateTube(List<Vector3> points)
    {
        if (points == null || points.Count < 2) return;

        int n = points.Count;
        int vertsPerRing = radialSegments + 1;
        List<Vector3> verts = new();
        List<int> tris = new();
        List<Vector2> uvs = new();

        for (int i = 0; i < n; i++)
        {
            Vector3 center = points[i];
            Vector3 forward = (i < n - 1) ? (points[i + 1] - points[i]).normalized
                                          : (points[i] - points[i - 1]).normalized;
            Vector3 side = Vector3.Cross(forward, Vector3.up);
            if (side.sqrMagnitude < 0.001f) side = Vector3.Cross(forward, Vector3.right);
            Vector3 up = Vector3.Cross(side, forward).normalized;

            Quaternion rot = Quaternion.LookRotation(forward, up);

            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = (j / (float)radialSegments) * Mathf.PI * 2f;
                Vector3 local = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                verts.Add(center + rot * local);
                uvs.Add(new Vector2(j / (float)radialSegments, i / (float)n));
            }
        }

        for (int i = 0; i < n - 1; i++)
        {
            int ringStart = i * vertsPerRing;
            int nextRingStart = (i + 1) * vertsPerRing;

            for (int j = 0; j < radialSegments; j++)
            {
                int a = ringStart + j;
                int b = ringStart + j + 1;
                int c = nextRingStart + j;
                int d = nextRingStart + j + 1;
                tris.AddRange(new int[] { a, c, b, b, c, d });
            }
        }



        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }

    public void ResetMesh()
    {
        mesh = new Mesh();
        if (meshFilter != null)
            meshFilter.mesh = mesh;
    }

}
