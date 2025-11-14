using UnityEngine;
using System.Collections;
using GogoGaga.OptimizedRopesAndCables;

public class RopeThreadController : MonoBehaviour
{
    public Rope rope;
    public RopeConfig config;
    public Transform anchor;
    public Transform tip;

    private Vector3 tipPos;
    private LineRenderer lr;
    private Material runtimeMat;

    private void Awake()
    {
        lr = rope.GetComponent<LineRenderer>();

        // clone material để mỗi rope có màu riêng
        runtimeMat = Instantiate(config.ropeMaterial);
        lr.material = runtimeMat;
        lr.widthMultiplier = config.ropeWidth;
    }

    private void Start()
    {
        tipPos = tip.position;
    }

    public void SetThreadColor(Color color)
    {
        runtimeMat.SetColor("_Color", color);
    }


    public void ResetTip()
    {
        tip.position = anchor.position;
        tipPos = tip.position;
        ShowThread();
    }

    public void ShowThread() => lr.enabled = true;
    public void HideThread() => lr.enabled = false;

    public IEnumerator MoveTipTo(Vector3 target, float speed)
    {
        ShowThread();

        while ((tipPos - target).sqrMagnitude > 0.0001f)
        {
            tipPos = Vector3.MoveTowards(tipPos, target, speed * Time.deltaTime);
            tip.position = tipPos;
            UpdateAnchor();
            yield return null;
        }

        tip.position = target;
        tipPos = target;
        UpdateAnchor();
    }

    private void UpdateAnchor()
    {
        float dist = Vector3.Distance(anchor.position, tip.position);
        float swing = Mathf.Sin(Time.time * config.swingSpeed)
                     * config.swingAmount
                     * Mathf.Clamp01(dist * 0.1f);

        rope.StartPoint.position = anchor.position + anchor.right * swing;
    }

    public void SetAnchor(Transform newAnchor)
    {
        anchor = newAnchor;

        // cập nhật ngay vị trí vào Rope StartPoint
        if (rope != null && rope.StartPoint != null)
        {
            rope.StartPoint.position = anchor.position;
        }
    }

}
