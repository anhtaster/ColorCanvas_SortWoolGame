using UnityEngine;
using WeavingPuzzle.Data;
using WeavingPuzzle.Events;

namespace WeavingPuzzle.View
{
    public class ThreadVisualizer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private WeavingConfig config;

        public void Initialize(WeavingConfig configuration)
        {
            config = configuration;
            SetupLineRenderer();
        }

        private void SetupLineRenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();

            Material material = config.ThreadMaterial != null
                ? config.ThreadMaterial
                : new Material(Shader.Find("Sprites/Default"));

            lineRenderer.material = material;
            lineRenderer.startWidth = config.ThreadWidth;
            lineRenderer.endWidth = config.ThreadWidth;
            lineRenderer.positionCount = 0;
            lineRenderer.sortingOrder = 10;
            lineRenderer.useWorldSpace = true;
        }

        public void ShowThread(Color threadColor)
        {
            lineRenderer.startColor = threadColor;
            lineRenderer.endColor = threadColor;
            lineRenderer.positionCount = 2;
        }

        public void HideThread()
        {
            lineRenderer.positionCount = 0;
        }

        public void UpdateThreadPosition(Vector3 startPos, Vector3 endPos)
        {
            if (lineRenderer.positionCount == 2)
            {
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
                WeavingEvents.ThreadMoved(endPos);
            }
        }
    }
}
