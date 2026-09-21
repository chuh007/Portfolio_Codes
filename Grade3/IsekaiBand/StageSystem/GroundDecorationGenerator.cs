using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal static class GroundDecorationGenerator
    {
        private const int SpawnChannel = 101;
        private const int OffsetXChannel = 211;
        private const int OffsetYChannel = 307;
        private const int RuleChannel = 401;
        private const int FlipChannel = 503;

        public static void Generate(
            Vector2Int chunkCoord,
            Vector2Int chunkTileSize,
            int stageSeed,
            StageDecorationProfileSO profile,
            Vector2 startPosition,
            Predicate<Vector3Int> canPlace,
            List<GroundDecorationPlacement> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            results.Clear();
            if (profile == null || !profile.HasGroundDecorations)
                return;

            int spacing = profile.Spacing;
            Vector3Int chunkStart = DecorationPlacementMath.GetChunkStartCell(chunkCoord, chunkTileSize);
            int minBlockX = DecorationPlacementMath.FloorDivide(chunkStart.x, spacing);
            int minBlockY = DecorationPlacementMath.FloorDivide(chunkStart.y, spacing);
            int maxBlockX = DecorationPlacementMath.FloorDivide(chunkStart.x + chunkTileSize.x - 1, spacing);
            int maxBlockY = DecorationPlacementMath.FloorDivide(chunkStart.y + chunkTileSize.y - 1, spacing);
            int decorationSeed = unchecked(stageSeed ^ profile.SeedOffset);
            float safeRadiusSqr = profile.StartSafeRadius * profile.StartSafeRadius;

            for (int blockX = minBlockX; blockX <= maxBlockX; blockX++)
            {
                for (int blockY = minBlockY; blockY <= maxBlockY; blockY++)
                {
                    uint spawnRoll = DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, SpawnChannel);
                    if (!DecorationPlacementMath.PassesChance(spawnRoll, profile.SpawnChancePerBlock))
                        continue;

                    int cellX = blockX * spacing
                                + (int)(DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, OffsetXChannel) % (uint)spacing);
                    int cellY = blockY * spacing
                                + (int)(DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, OffsetYChannel) % (uint)spacing);

                    if (cellX < chunkStart.x || cellX >= chunkStart.x + chunkTileSize.x
                        || cellY < chunkStart.y || cellY >= chunkStart.y + chunkTileSize.y)
                    {
                        continue;
                    }

                    Vector3Int cell = new Vector3Int(cellX, cellY, 0);
                    Vector2 cellCenter = new Vector2(cellX + 0.5f, cellY + 0.5f);
                    if (safeRadiusSqr > 0f && (cellCenter - startPosition).sqrMagnitude < safeRadiusSqr)
                        continue;

                    if (canPlace != null && !canPlace(cell))
                        continue;

                    GroundDecorationRule rule = profile.SelectGroundDecoration(
                        DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, RuleChannel));
                    if (rule == null)
                        continue;

                    bool flipX = rule.AllowFlipX
                                 && (DecorationPlacementMath.Hash(blockX, blockY, decorationSeed, FlipChannel) & 1u) != 0u;
                    results.Add(new GroundDecorationPlacement(cell, rule.Tile, flipX));
                }
            }
        }
    }
}
