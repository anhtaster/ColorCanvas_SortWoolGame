using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeavingPuzzle.Data;
using WeavingPuzzle.Model;
using WeavingPuzzle.View;
using WeavingPuzzle.Events;

namespace WeavingPuzzle.Controller
{
    public class WeavingController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private WeavingConfig config;

        [Header("Layer Data")]
        [SerializeField] private WeavingLayerData yellowLayer;
        [SerializeField] private WeavingLayerData pinkLayer;
        [SerializeField] private WeavingLayerData brownLayer;

        [Header("Components")]
        [SerializeField] private WeavingCanvas canvas;
        [SerializeField] private ThreadVisualizer threadVisualizer;
        [SerializeField] private WeavingUIView uiView;

        private IPathGenerator pathGenerator;
        private bool isWeaving = false;

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            pathGenerator = new LinearPathGenerator();

            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("WeavingCanvas");
                canvasObj.transform.SetParent(transform);
                canvas = canvasObj.AddComponent<WeavingCanvas>();
            }
            canvas.Initialize(config);

            if (threadVisualizer == null)
            {
                GameObject threadObj = new GameObject("ThreadVisualizer");
                threadObj.transform.SetParent(transform);
                threadVisualizer = threadObj.AddComponent<ThreadVisualizer>();
            }
            threadVisualizer.Initialize(config);

            if (uiView != null)
            {
                uiView.Initialize(yellowLayer, pinkLayer, brownLayer);
                uiView.OnWeaveRequested += HandleWeaveRequest;
                uiView.OnResetRequested += HandleResetRequest;
            }
        }

        private void HandleWeaveRequest(WeavingLayerData layerData)
        {
            if (!isWeaving && layerData != null)
            {
                StartCoroutine(WeaveLayerRoutine(layerData));
            }
        }

        private void HandleResetRequest()
        {
            if (!isWeaving)
            {
                canvas.ResetCanvas();
            }
        }

        private IEnumerator WeaveLayerRoutine(WeavingLayerData layerData)
        {
            isWeaving = true;
            uiView?.SetButtonsInteractable(false);

            WeavingEvents.WeavingStarted();

            threadVisualizer.ShowThread(layerData.ThreadColor);

            List<Vector2Int> path = pathGenerator.GeneratePath(config.GridSize);
            Vector3 previousPosition = Vector3.zero;

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int gridPos = path[i];
                CanvasBlock block = canvas.GetBlock(gridPos.x, gridPos.y);

                if (block != null)
                {
                    Vector3 blockPosition = block.WorldPosition;

                    threadVisualizer.UpdateThreadPosition(previousPosition, blockPosition);

                    int spriteY = config.GridSize - 1 - gridPos.y;
                    if (layerData.IsPixelVisible(gridPos.x, spriteY, config.AlphaThreshold))
                    {
                        Color pixelColor = layerData.GetPixelColor(gridPos.x, spriteY);
                        canvas.ColorBlock(gridPos.x, gridPos.y, pixelColor);
                    }

                    previousPosition = blockPosition;

                    float waitTime = 1f / config.ThreadSpeed;
                    yield return new WaitForSeconds(waitTime);
                }
            }

            threadVisualizer.HideThread();

            WeavingEvents.WeavingCompleted();

            isWeaving = false;
            uiView?.SetButtonsInteractable(true);
        }

        private void OnDestroy()
        {
            if (uiView != null)
            {
                uiView.OnWeaveRequested -= HandleWeaveRequest;
                uiView.OnResetRequested -= HandleResetRequest;
            }
        }
    }
}
