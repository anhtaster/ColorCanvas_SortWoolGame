using System.Collections.Generic;
using UnityEngine;

namespace WeavingPuzzle.Model
{
    public interface IPathGenerator
    {
        List<Vector2Int> GeneratePath(int gridSize);
    }

    public class LinearPathGenerator : IPathGenerator
    {
        public List<Vector2Int> GeneratePath(int gridSize)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    path.Add(new Vector2Int(col, row));
                }
            }

            return path;
        }
    }

    public class SerpentinePathGenerator : IPathGenerator
    {
        public List<Vector2Int> GeneratePath(int gridSize)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            for (int row = 0; row < gridSize; row++)
            {
                if (row % 2 == 0)
                {
                    for (int col = 0; col < gridSize; col++)
                    {
                        path.Add(new Vector2Int(col, row));
                    }
                }
                else
                {
                    for (int col = gridSize - 1; col >= 0; col--)
                    {
                        path.Add(new Vector2Int(col, row));
                    }
                }
            }

            return path;
        }
    }
}
