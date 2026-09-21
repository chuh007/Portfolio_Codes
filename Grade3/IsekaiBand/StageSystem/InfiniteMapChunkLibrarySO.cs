using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    [CreateAssetMenu(fileName = "InfiniteMapChunkLibrary", menuName = "SO/Stage/Infinite Map Chunk Library", order = 3)]
    public class InfiniteMapChunkLibrarySO : ScriptableObject
    {
        [SerializeField] private int seed = 1597334677;
        [SerializeField, Min(1)] private int chunkWidth = InfiniteMapChunkDataSO.DefaultSize;
        [SerializeField, Min(1)] private int chunkHeight = InfiniteMapChunkDataSO.DefaultSize;
        [SerializeField] private List<InfiniteMapChunkDataSO> chunks = new();

        public int Seed => seed;
        public Vector2Int ChunkTileSize => new Vector2Int(Mathf.Max(1, chunkWidth), Mathf.Max(1, chunkHeight));
        public IReadOnlyList<InfiniteMapChunkDataSO> Chunks => chunks;

        public bool HasChunks
        {
            get
            {
                foreach (InfiniteMapChunkDataSO chunk in chunks)
                {
                    if (IsUsableChunk(chunk))
                        return true;
                }

                return false;
            }
        }

        public bool IsUsableChunk(InfiniteMapChunkDataSO chunk)
        {
            return chunk != null && chunk.HasSameSize(ChunkTileSize);
        }

        public void AddChunk(InfiniteMapChunkDataSO chunk)
        {
            if (chunk == null || chunks.Contains(chunk))
                return;

            chunks.Add(chunk);
        }

        private void OnValidate()
        {
            chunkWidth = Mathf.Max(1, chunkWidth);
            chunkHeight = Mathf.Max(1, chunkHeight);

            if (chunks == null)
                chunks = new List<InfiniteMapChunkDataSO>();
        }
    }
}
