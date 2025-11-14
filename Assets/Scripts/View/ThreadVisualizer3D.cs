using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WeavingPuzzle.Data;

public class ThreadVisualizer3D : MonoBehaviour
{
    private ThreadMesh3D meshTube;
    private WeavingConfig config;

    private Transform anchor;
    private Vector3 tipPos;
    private float oscTime;

    private List<Vector3> curvePoints = new();

    public void Initialize(WeavingConfig configuration)
    {
        config = configuration;
        meshTube = gameObject.AddComponent<ThreadMesh3D>();
        meshTube.SetMaterial(config.ThreadMaterial);
        meshTube.radius = config.ThreadWidth * 0.5f;
    }

    public void AttachTo(Transform spoolAnchor, Color color)
    {
        anchor = spoolAnchor;
        tipPos = anchor.position;
        meshTube.SetMaterial(new Material(config.ThreadMaterial));

        MeshRenderer renderer = meshTube.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        meshTube.ResetMesh();
    }

    public IEnumerator MoveTipTo(Vector3 target, float speed)
    {
        target.z += 0.02f;
        while ((tipPos - target).sqrMagnitude > 0.0001f)
        {
            tipPos = Vector3.MoveTowards(tipPos, target, speed * Time.deltaTime);
            RebuildCurve();
            yield return null;
        }
        tipPos = target;
        RebuildCurve();
    }

    private void RebuildCurve()
    {
        if (anchor == null) return;

        oscTime += Time.deltaTime * 6f;

        Vector3 p0 = anchor.position + anchor.right * Mathf.Sin(oscTime) * 0.12f;
        Vector3 p3 = tipPos;
        Vector3 ab = p3 - p0;
        Vector3 up = Vector3.up;

        float sag = Mathf.Clamp(ab.magnitude * 0.2f, 0f, 1.5f);
        Vector3 p1 = p0 + ab * 0.25f + up * sag;
        Vector3 p2 = p0 + ab * 0.75f + up * sag * 0.4f;

        int samples = 32;
        curvePoints.Clear();
        for (int i = 0; i < samples; i++)
        {
            float t = i / (samples - 1f);
            curvePoints.Add(EvaluateCubic(p0, p1, p2, p3, t));
        }

        meshTube.GenerateTube(curvePoints);
    }

    private Vector3 EvaluateCubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
    }

    public void HideThread()
    {
        if (meshTube != null)
        {
            MeshFilter meshFilter = meshTube.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.mesh != null)
            {
                meshFilter.mesh.Clear();
            }
        }
    }
}
