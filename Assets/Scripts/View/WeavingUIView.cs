using System;
using UnityEngine;
using UnityEngine.UI;
using WeavingPuzzle.Data;

namespace WeavingPuzzle.View
{
    public class WeavingUIView : MonoBehaviour
    {
        [SerializeField] private Button weaveYellowButton;
        [SerializeField] private Button weavePinkButton;
        [SerializeField] private Button weaveBrownButton;
        [SerializeField] private Button resetButton;

        public event Action<WeavingLayerData> OnWeaveRequested;
        public event Action OnResetRequested;

        private WeavingLayerData yellowLayer;
        private WeavingLayerData pinkLayer;
        private WeavingLayerData brownLayer;

        public void Initialize(WeavingLayerData yellow, WeavingLayerData pink, WeavingLayerData brown)
        {
            yellowLayer = yellow;
            pinkLayer = pink;
            brownLayer = brown;

            SetupButtons();
        }

        private void SetupButtons()
        {
            if (weaveYellowButton != null)
                weaveYellowButton.onClick.AddListener(() => OnWeaveRequested?.Invoke(yellowLayer));

            if (weavePinkButton != null)
                weavePinkButton.onClick.AddListener(() => OnWeaveRequested?.Invoke(pinkLayer));

            if (weaveBrownButton != null)
                weaveBrownButton.onClick.AddListener(() => OnWeaveRequested?.Invoke(brownLayer));

            if (resetButton != null)
                resetButton.onClick.AddListener(() => OnResetRequested?.Invoke());
        }

        public void SetButtonsInteractable(bool interactable)
        {
            if (weaveYellowButton != null) weaveYellowButton.interactable = interactable;
            if (weavePinkButton != null) weavePinkButton.interactable = interactable;
            if (weaveBrownButton != null) weaveBrownButton.interactable = interactable;
            if (resetButton != null) resetButton.interactable = interactable;
        }

        private void OnDestroy()
        {
            if (weaveYellowButton != null) weaveYellowButton.onClick.RemoveAllListeners();
            if (weavePinkButton != null) weavePinkButton.onClick.RemoveAllListeners();
            if (weaveBrownButton != null) weaveBrownButton.onClick.RemoveAllListeners();
            if (resetButton != null) resetButton.onClick.RemoveAllListeners();
        }
    }
}
