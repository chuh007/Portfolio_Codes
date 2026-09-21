using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    public static class DecorationPlacementGenerator
    {
        public static void GenerateGroundDecorations(
            Vector2Int chunkCoord,
            Vector2Int chunkTileSize,
            int stageSeed,
            StageDecorationProfileSO profile,
            Vector2 startPosition,
            Predicate<Vector3Int> canPlace,
            List<GroundDecorationPlacement> results)
            => GroundDecorationGenerator.Generate(
                chunkCoord, chunkTileSize, stageSeed, profile, startPosition, canPlace, results);

        public static void GenerateLargeDecorations(
            Vector2Int chunkCoord,
            Vector2Int chunkTileSize,
            int stageSeed,
            StageDecorationProfileSO profile,
            Vector2 startPosition,
            Predicate<Vector3Int> canPlace,
            List<LargeDecorationPlacement> results)
            => LargeDecorationGenerator.Generate(
                chunkCoord, chunkTileSize, stageSeed, profile, startPosition, canPlace, results);
    }
}
