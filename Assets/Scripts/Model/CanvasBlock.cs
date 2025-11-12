using UnityEngine;

namespace WeavingPuzzle.Model
{
    public class CanvasBlock : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private int gridX;
        private int gridY;

        public int GridX => gridX;
        public int GridY => gridY;
        public Vector3 WorldPosition => transform.position;

        public void Initialize(int x, int y, Sprite sprite, Color initialColor)
        {
            gridX = x;
            gridY = y;

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = initialColor;
        }

        public void SetColor(Color color)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        public Color GetColor()
        {
            return spriteRenderer != null ? spriteRenderer.color : Color.white;
        }

        public void ResetToDefault(Color defaultColor)
        {
            SetColor(defaultColor);
        }
    }
}
