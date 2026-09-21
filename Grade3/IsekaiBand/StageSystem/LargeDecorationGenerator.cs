using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal static class LargeDecorationGenerator
    {
        private const int LargeSpawnChannel = 601;
        private const int LargeOffsetXChannel = 701;
        private const int LargeOffsetYChannel = 809;
        private const int LargeRuleChannel = 907;
        private const int LargeFlipChannel = 1009;

        public static void Generate(
            Vector2Int chunkCoord,
            Vector2Int chunkTileSize,
            int stageSeed,
            StageDecorationProfileSO profile,
            Vector2 startPosition,
            Predicate<Vector3Int> canPlace,
            List<LargeDecorationPlacement> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            results.Clear();
            if (profile == null || !profile.HasLargeDecorations)
                return;

            int spacing = profile.LargeSpacing;
            int padding = profile.LargeBlockPadding;
            int candidateSpan = Mathf.Max(1, spacing - padding * 2);
            Vector3Int chunkStart = DecorationPlacementMath.GetChunkStartCell(chunkCoord, chunkTileSize);
            int minBlockX = DecorationPlacementMath.FloorDivide(chunkStart.x, spacing);
            int minBlockY = DecorationPlacementMath.FloorDivide(chunkStart.y, spacing);
            int maxBlockX = DecorationPlacementMath.FloorDivide(chunkStart.x + chunkTileSize.x - 1, spacing);
            int maxBlockY = DecorationPlacementMath.FloorDivide(chunkStart.y + chunkTileSize.y - 1, spacing);
            int decorationSeed = unchecked(stageSeed ^ profile.SeedOffset);
            float safeRadiusSqr = profile.LargeStartSafeRadius * profile.LargeStartSafeRadius;

            for (int blockX = minBlockX; blockX <= maxBlockX; blockX++)
            {
                for (int blockY = minBlockY; blockY <= maxBlockY; blockY++)
                {
                    uint spawnRoll = DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, LargeSpawnChannel);
                    if (!DecorationPlacementMath.PassesChance(spawnRoll, profile.LargeSpawnChancePerBlock))
                        continue;

                    int cellX = blockX * spacing + padding
                                + (int)(DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, LargeOffsetXChannel)
                                        % (uint)candidateSpan);
                    int cellY = blockY * spacing + padding
                                + (int)(DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, LargeOffsetYChannel)
                                        % (uint)candidateSpan);

                    if (cellX < chunkStart.x || cellX >= chunkStart.x + chunkTileSize.x
                        || cellY < chunkStart.y || cellY >= chunkStart.y + chunkTileSize.y)
                    {
                        continue;
                    }

                    Vector3Int cell = new Vector3Int(cellX, cellY, 0);
                    Vector2 cellCenter = new Vector2(cellX + 0.5f, cellY + 0.5f);
                    if (safeRadiusSqr > 0f && (cellCenter - startPosition).sqrMagnitude < safeRadiusSqr)
                        continue;

                    LargeDecorationRule rule = profile.SelectLargeDecoration(
                        DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, LargeRuleChannel));
                    if (rule == null || !DecorationPlacementMath.HasTerrainClearance(cell, rule.TerrainClearanceRadius, canPlace))
                        continue;

                    bool flipX = rule.AllowFlipX
                                 && (DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, LargeFlipChannel) & 1u) != 0u;
                    results.Add(new LargeDecorationPlacement(cell, rule.Prefab, flipX));
                }
            }
        }
    }
}
