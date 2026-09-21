using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class InfiniteMapChunkSelection
    {
        private readonly Dictionary<Vector2Int, InfiniteMapChunkDataSO> _chunkSelections = new();
        private StageDataSO _stageData;
        private bool _missingUsableChunkWarningShown;

        public void Setup(StageDataSO stageData)
        {
            _stageData = stageData;
            _chunkSelections.Clear();
            _missingUsableChunkWarningShown = false;
        }

        public InfiniteMapChunkDataSO Select(Vector2Int coord)
        {
            InfiniteMapChunkLibrarySO library = _stageData.ChunkLibrary;
            if (_chunkSelections.TryGetValue(coord, out InfiniteMapChunkDataSO selected)
                && library.IsUsableChunk(selected)
                && selected.SpawnWeight > 0)
            {
                return selected;
            }

            int totalWeight = 0;
            foreach (InfiniteMapChunkDataSO chunk in library.Chunks)
            {
                if (library.IsUsableChunk(chunk) && chunk.SpawnWeight > 0)
                    totalWeight += chunk.SpawnWeight;
            }

            if (totalWeight == 0)
            {
                if (!_missingUsableChunkWarningShown)
                {
                    Debug.LogWarning("[InfiniteMapChunkRenderer] No usable weighted chunk was found. Skipping chunk generation.");
                    _missingUsableChunkWarningShown = true;
                }

                return null;
            }

            int targetWeight = (int)(InfiniteMapChunkLayout.Hash(coord.x, coord.y, library.Seed) % (uint)totalWeight);
            int currentWeight = 0;
            foreach (InfiniteMapChunkDataSO chunk in library.Chunks)
            {
                if (!library.IsUsableChunk(chunk) || chunk.SpawnWeight <= 0)
                    continue;

                currentWeight += chunk.SpawnWeight;
                if (targetWeight < currentWeight)
                {
                    selected = chunk;
                    break;
                }
            }

            if (selected == null)
                return null;

            _chunkSelections[coord] = selected;
            return selected;
        }
    }
}
