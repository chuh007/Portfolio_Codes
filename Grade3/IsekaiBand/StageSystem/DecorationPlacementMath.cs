using System;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal static class DecorationPlacementMath
    {
        public static bool HasTerrainClearance(
            Vector3Int center,
            int clearanceRadius,
            Predicate<Vector3Int> canPlace)
        {
            if (canPlace == null)
                return true;

            for (int x = -clearanceRadius; x <= clearanceRadius; x++)
            {
                for (int y = -clearanceRadius; y <= clearanceRadius; y++)
                {
                    if (!canPlace(new Vector3Int(center.x + x, center.y + y, center.z)))
                        return false;
                }
            }

            return true;
        }

        public static Vector3Int GetChunkStartCell(Vector2Int chunkCoord, Vector2Int chunkTileSize)
        {
            return new Vector3Int(
                chunkCoord.x * chunkTileSize.x - chunkTileSize.x / 2,
                chunkCoord.y * chunkTileSize.y - chunkTileSize.y / 2,
                0);
        }

        public static int FloorDivide(int value, int divisor)
        {
            int quotient = value / divisor;
            int remainder = value % divisor;
            return remainder < 0 ? quotient - 1 : quotient;
        }

        public static bool PassesChance(uint roll, float chance)
        {
            if (chance <= 0f)
                return false;

            if (chance >= 1f)
                return true;

            const uint resolution = 1_000_000u;
            return roll % resolution < (uint)(chance * resolution);
        }

        public static uint Hash(int x, int y, int seed, int channel)
        {
            unchecked
            {
                uint hash = (uint)seed ^ ((uint)channel * 2246822519u);
                hash ^= (uint)x * 374761393u;
                hash = (hash << 13) | (hash >> 19);
                hash ^= (uint)y * 668265263u;
                hash *= 1274126177u;
                return hash ^ (hash >> 16);
            }
        }
    }
}
