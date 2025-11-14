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

        [Header("Rope System (New)")]
        [SerializeField] private RopeThreadController ropeThread;   // NEW
        [SerializeField] private Transform spoolAnchor;             // Anchor where rope starts

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

            // NEW: RopeThreadController was previously ThreadVisualizer3D
            if (ropeThread == null)
            {
                Debug.LogError("RopeThreadController is missing. Assign RopeObject in Inspector.");
            }

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

            // NEW: Attach rope to spool anchor
            ropeThread.SetAnchor(spoolAnchor);

            ropeThread.SetThreadColor(layerData.ThreadColor);


            // Create path
            IPathGenerator pathGenerator = new SelectivePathGenerator(layerData, config);
            var path = pathGenerator.GeneratePath(config.GridSize);

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int gridPos = path[i];
                CanvasBlock block = canvas.GetBlock(gridPos.x, gridPos.y);
                if (block == null) continue;

                // Move tip of rope to cell (just like old MoveTipTo)
                yield return ropeThread.MoveTipTo(block.WorldPosition, config.ThreadSpeed);

                // Color pixel if needed
                int spriteY = config.GridSize - 1 - gridPos.y;
                if (layerData.IsPixelVisible(gridPos.x, spriteY, config.AlphaThreshold))
                {
                    Color pixelColor = layerData.GetPixelColor(gridPos.x, spriteY);
                    canvas.ColorBlock(gridPos.x, gridPos.y, pixelColor);
                }
            }

            ropeThread.HideThread();
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
