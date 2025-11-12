using UnityEngine;

namespace WeavingPuzzle.Data
{
    [CreateAssetMenu(fileName = "WeavingConfig", menuName = "Weaving Puzzle/Configuration")]
    public class WeavingConfig : ScriptableObject
    {
        [Header("Canvas Settings")]
        [SerializeField] private int gridSize = 26;
        [SerializeField] private float blockSize = 0.2f;
        [SerializeField] private Color defaultBlockColor = Color.white;

        [Header("Thread Settings")]
        [SerializeField] private float threadSpeed = 50f;
        [SerializeField] private float threadWidth = 0.1f;
        [SerializeField] private float alphaThreshold = 0.5f;

        [Header("Visual Settings")]
        [SerializeField] private Material blockMaterial;
        [SerializeField] private Material threadMaterial;

        public int GridSize => gridSize;
        public float BlockSize => blockSize;
        public Color DefaultBlockColor => defaultBlockColor;
        public float ThreadSpeed => threadSpeed;
        public float ThreadWidth => threadWidth;
        public float AlphaThreshold => alphaThreshold;
        public Material BlockMaterial => blockMaterial;
        public Material ThreadMaterial => threadMaterial;
    }
}
