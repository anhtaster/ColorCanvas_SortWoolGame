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

    private ThreadRope rope = new ThreadRope();

    // Điều chỉnh rope
    [Header("Rope Physics Settings")]
    public int ropeSegments = 16;
    public float segmentLength = 0.08f;
    public float threadFollowSpeed = 8f;

    private List<Vector3> curvePoints = new List<Vector3>();

    // ---------------------------------------------------------
    public void Initialize(WeavingConfig configuration)
    {
        config = configuration;

        meshTube = GetComponent<ThreadMesh3D>();
        if (meshTube == null)
            meshTube = gameObject.AddComponent<ThreadMesh3D>();

        meshTube.radius = config.ThreadWidth * 0.5f;
        meshTube.SetMaterial(config.ThreadMaterial);
    }

    // ---------------------------------------------------------
    public void AttachTo(Transform spoolAnchor, Color color)
    {
        anchor = spoolAnchor;
        tipPos = anchor.position;

        // khởi tạo rope với segment
        rope.Initialize(anchor.position, ropeSegments, segmentLength);

        meshTube.gameObject.SetActive(true);
        meshTube.ResetMesh();

        Material mat = new Material(config.ThreadMaterial);
        mat.color = color;
        meshTube.SetMaterial(mat);
    }

    // ---------------------------------------------------------
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

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }


    // ---------------------------------------------------------
    private void RebuildCurve()
    {
        if (anchor == null) return;

        float dist = Vector3.Distance(anchor.position, tipPos);

        // Swing anchor
        float swing = Mathf.Sin(Time.time * (6f + dist * 0.5f)) * (0.05f + dist * 0.02f);
        Vector3 anchorPos = anchor.position + anchor.right * swing;

        // Rope Physics
        rope.Simulate(anchorPos, tipPos);

        // RENDER BẰNG SPLINE
        curvePoints.Clear();

        for (int i = 0; i < rope.nodes.Count - 3; i++)
        {
            Vector3 p0 = rope.nodes[i].current;
            Vector3 p1 = rope.nodes[i + 1].current;
            Vector3 p2 = rope.nodes[i + 2].current;
            Vector3 p3 = rope.nodes[i + 3].current;

            // phân nhỏ mỗi đoạn spline thành nhiều mẫu
            for (int j = 0; j < 6; j++)
            {
                float t = j / 6f;
                curvePoints.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        meshTube.GenerateTube(curvePoints);
    }


    // ---------------------------------------------------------
    public void HideThread()
    {
        meshTube.gameObject.SetActive(false);
    }
}
