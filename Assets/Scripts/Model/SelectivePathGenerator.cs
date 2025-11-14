using System.Collections.Generic;
using UnityEngine;
using WeavingPuzzle.Data;

namespace WeavingPuzzle.Model
{
    public class SelectivePathGenerator : IPathGenerator
    {
        private readonly WeavingLayerData layerData;
        private readonly WeavingConfig config;

        public SelectivePathGenerator(WeavingLayerData data, WeavingConfig cfg)
        {
            layerData = data;
            config = cfg;
        }

        public List<Vector2Int> GeneratePath(int gridSize)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    int spriteY = gridSize - 1 - y;
                    if (layerData.IsPixelVisible(x, spriteY, config.AlphaThreshold))
                        path.Add(new Vector2Int(x, y));
                }
            }

            return path;
        }
    }
}
