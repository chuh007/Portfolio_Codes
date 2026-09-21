using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    [CreateAssetMenu(fileName = "InfiniteMapChunk", menuName = "SO/Stage/Infinite Map Chunk", order = 2)]
    public class InfiniteMapChunkDataSO : ScriptableObject
    {
        public const int DefaultSize = 8;

        [SerializeField, Min(1)] private int width = DefaultSize;
        [SerializeField, Min(1)] private int height = DefaultSize;
        [SerializeField, Min(0)] private int spawnWeight = 1;
        [SerializeField] private List<TileBase> tiles = new();

        public int Width => Mathf.Max(1, width);
        public int Height => Mathf.Max(1, height);
        public int SpawnWeight => Mathf.Max(0, spawnWeight);
        public Vector2Int Size => new Vector2Int(Width, Height);

        public TileBase GetTile(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            EnsureTileCount();
            return tiles[ToIndex(x, y)];
        }

        public void SetTile(int x, int y, TileBase tile)
        {
            if (!IsInBounds(x, y))
                return;

            EnsureTileCount();
            tiles[ToIndex(x, y)] = tile;
        }

        public void SetSpawnWeight(int weight)
        {
            spawnWeight = Mathf.Max(0, weight);
        }

        public void Resize(int newWidth, int newHeight)
        {
            newWidth = Mathf.Max(1, newWidth);
            newHeight = Mathf.Max(1, newHeight);

            List<TileBase> resizedTiles = new List<TileBase>(newWidth * newHeight);
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    resizedTiles.Add(x < Width && y < Height ? GetTile(x, y) : null);
                }
            }

            width = newWidth;
            height = newHeight;
            tiles = resizedTiles;
        }

        public bool HasSameSize(Vector2Int targetSize)
        {
            return Width == targetSize.x && Height == targetSize.y;
        }

        public bool Validate(out List<string> messages)
        {
            EnsureTileCount();
            messages = new List<string>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (GetTile(x, y) == null)
                        messages.Add($"Missing tile at ({x}, {y}).");
                }
            }

            return messages.Count == 0;
        }

        private void OnValidate()
        {
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
            spawnWeight = Mathf.Max(0, spawnWeight);
            EnsureTileCount();
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private int ToIndex(int x, int y)
        {
            return y * Width + x;
        }

        private void EnsureTileCount()
        {
            int targetCount = Width * Height;
            if (tiles == null)
                tiles = new List<TileBase>(targetCount);

            while (tiles.Count < targetCount)
                tiles.Add(null);

            if (tiles.Count > targetCount)
                tiles.RemoveRange(targetCount, tiles.Count - targetCount);
        }
    }
}
