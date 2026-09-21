using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class ItemChestSpawnPosition
    {
        public static Vector2 RandomItemChestSpawnPosition(SpawnerPositionContext context, float radius, float padding)
        {
            Vector2 center = context.PlayerPosition;

            if (context.IsInfiniteMap || !context.HasMapBounds)
                return center + Random.insideUnitCircle * radius;

            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = center + Random.insideUnitCircle * radius;
                if (SpawnMapBounds.IsInMapBounds(context, pos, padding))
                    return pos;
            }

            return SpawnMapBounds.ClampToMapBounds(context, center, padding);
        }

        public static Vector2 RandomItemChestSpawnPosition(
            SpawnerPositionContext context,
            float minRadius,
            float maxRadius,
            float padding)
        {
            Vector2 center = context.PlayerPosition;
            float min = Mathf.Min(minRadius, maxRadius);
            float max = Mathf.Max(minRadius, maxRadius);

            if (context.IsInfiniteMap || !context.HasMapBounds)
                return center + SpawnDirection.RandomDirection() * Random.Range(min, max);

            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = center + SpawnDirection.RandomDirection() * Random.Range(min, max);
                if (SpawnMapBounds.IsInMapBounds(context, pos, padding))
                    return pos;
            }

            return SpawnMapBounds.ClampToMapBounds(context, center + SpawnDirection.RandomDirection() * min, padding);
        }
    }
}
