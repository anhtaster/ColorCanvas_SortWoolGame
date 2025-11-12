using UnityEngine;

namespace WeavingPuzzle.Data
{
    [CreateAssetMenu(fileName = "WeavingLayer", menuName = "Weaving Puzzle/Layer Data")]
    public class WeavingLayerData : ScriptableObject
    {
        [SerializeField] private string layerName;
        [SerializeField] private Texture2D spriteTexture;
        [SerializeField] private Color threadColor;
        [SerializeField] private int orderIndex;

        public string LayerName => layerName;
        public Texture2D SpriteTexture => spriteTexture;
        public Color ThreadColor => threadColor;
        public int OrderIndex => orderIndex;

        public Color GetPixelColor(int x, int y)
        {
            if (spriteTexture == null) return Color.clear;
            return spriteTexture.GetPixel(x, y);
        }

        public bool IsPixelVisible(int x, int y, float alphaThreshold = 0.5f)
        {
            Color pixel = GetPixelColor(x, y);
            return pixel.a > alphaThreshold;
        }
    }
}
