using UnityEngine;
using WeavingPuzzle.Data;
using WeavingPuzzle.Events;

namespace WeavingPuzzle.Model
{
    public class WeavingCanvas : MonoBehaviour
    {
        private WeavingConfig config;
        private CanvasBlock[,] blocks;
        private Sprite blockSprite;

        public void Initialize(WeavingConfig configuration)
        {
            config = configuration;
            CreateBlockSprite();
            GenerateCanvas();
        }

        private void CreateBlockSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            blockSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private void GenerateCanvas()
        {
            blocks = new CanvasBlock[config.GridSize, config.GridSize];

            float canvasWidth = config.GridSize * config.BlockSize;
            float canvasHeight = config.GridSize * config.BlockSize;
            Vector3 startPosition = new Vector3(-canvasWidth / 2f, canvasHeight / 2f, 0f);

            for (int y = 0; y < config.GridSize; y++)
            {
                for (int x = 0; x < config.GridSize; x++)
                {
                    GameObject blockObj = new GameObject($"Block_{x}_{y}");
                    blockObj.transform.SetParent(transform);

                    float xPos = startPosition.x + (x * config.BlockSize) + (config.BlockSize / 2f);
                    float yPos = startPosition.y - (y * config.BlockSize) - (config.BlockSize / 2f);
                    blockObj.transform.position = new Vector3(xPos, yPos, 0f);
                    blockObj.transform.localScale = Vector3.one * config.BlockSize;

                    CanvasBlock block = blockObj.AddComponent<CanvasBlock>();
                    block.Initialize(x, y, blockSprite, config.DefaultBlockColor);

                    blocks[x, y] = block;
                }
            }
        }

        public CanvasBlock GetBlock(int x, int y)
        {
            if (x >= 0 && x < config.GridSize && y >= 0 && y < config.GridSize)
            {
                return blocks[x, y];
            }
            return null;
        }

        public void ColorBlock(int x, int y, Color color)
        {
            CanvasBlock block = GetBlock(x, y);
            if (block != null)
            {
                block.SetColor(color);
                WeavingEvents.BlockColored(x, y);
            }
        }

        public void ResetCanvas()
        {
            for (int y = 0; y < config.GridSize; y++)
            {
                for (int x = 0; x < config.GridSize; x++)
                {
                    blocks[x, y].ResetToDefault(config.DefaultBlockColor);
                }
            }
            WeavingEvents.CanvasReset();
        }

        public int GetGridSize()
        {
            return config.GridSize;
        }
    }
}
