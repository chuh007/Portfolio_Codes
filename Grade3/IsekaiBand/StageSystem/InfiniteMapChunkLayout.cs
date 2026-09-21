using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal readonly struct InfiniteMapChunkLayout
    {
        public InfiniteMapChunkLayout(StageDataSO stageData)
        {
            UsesLibrary = stageData.ChunkLibrary != null && stageData.ChunkLibrary.HasChunks;
            if (UsesLibrary)
            {
                TileSize = stageData.ChunkLibrary.ChunkTileSize;
                WorldSize = TileSize;
            }
            else
            {
                WorldSize = new Vector2(Mathf.Max(1f, stageData.StageSize.x), Mathf.Max(1f, stageData.StageSize.y));
                TileSize = new Vector2Int(
                    Mathf.Max(1, Mathf.CeilToInt(WorldSize.x)), Mathf.Max(1, Mathf.CeilToInt(WorldSize.y)));
            }
        }

        public bool UsesLibrary { get; }
        public Vector2 WorldSize { get; }
        public Vector2Int TileSize { get; }

        public Vector3Int StartCell(Vector2Int coord)
            => new Vector3Int(coord.x * TileSize.x - TileSize.x / 2, coord.y * TileSize.y - TileSize.y / 2, 0);

        public Vector2Int WorldToChunk(Vector2 position)
            => new Vector2Int(
                Mathf.FloorToInt((position.x + WorldSize.x * 0.5f) / WorldSize.x),
                Mathf.FloorToInt((position.y + WorldSize.y * 0.5f) / WorldSize.y));

        public static uint Hash(int x, int y, int seed)
        {
            unchecked
            {
                uint hash = (uint)seed;
                hash ^= (uint)x * 374761393u;
                hash = (hash << 13) | (hash >> 19);
                hash ^= (uint)y * 668265263u;
                hash *= 1274126177u;
                return hash ^ (hash >> 16);
            }
        }
    }
}
