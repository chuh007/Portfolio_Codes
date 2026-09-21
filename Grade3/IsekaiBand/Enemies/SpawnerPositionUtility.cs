using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class SpawnerPositionUtility
    {
        public static Vector2 RandomSpawnPosition(SpawnerPositionContext context)
        {
            Vector2 center = context.PlayerPosition;
            float min = Mathf.Min(context.MinDistance, context.MaxDistance);
            float max = Mathf.Max(context.MinDistance, context.MaxDistance);

            int cnt = 0;
            while (cnt++ < 10)
            {
                Vector2 randomDir = SpawnDirection.RandomDirection();
                Vector2 pos = SpawnCameraBounds.TryGetSpawnRectBandPosition(context, randomDir, out Vector2 bandPosition)
                    ? bandPosition
                    : center + randomDir * Random.Range(min, max);

                if (context.IsInfiniteMap || !context.HasMapBounds || SpawnMapBounds.IsInMapBounds(context, pos, 0f))
                    return pos;
            }

            if (context.IsInfiniteMap || !context.HasMapBounds)
                return SpawnMapBounds.ClampToMapBounds(context, center + SpawnDirection.RandomDirection() * min, 0f);

            return SpawnMapBounds.ClampToMapBounds(context, new Vector2(context.MapHalfWidth, context.MapHalfHeight), 0f);
        }

        public static Vector2 RandomDistantSpawnPosition(
            SpawnerPositionContext context,
            float distanceMultiplier)
        {
            Vector2 position = RandomSpawnPosition(context);
            float multiplier = Mathf.Max(1f, distanceMultiplier);
            if (multiplier <= 1f)
                return position;

            Vector2 center = context.CameraCenter;
            Vector2 distantPosition = center + (position - center) * multiplier;
            distantPosition = SpawnCameraBounds.ClampInsideDespawnBounds(context, distantPosition);
            return SpawnMapBounds.ClampToMapBounds(context, distantPosition, 0f);
        }

        public static Vector2 RandomForwardSpawnPosition(SpawnerPositionContext context)
        {
            Vector2 center = context.PlayerPosition;
            Vector2 forward = SpawnDirection.GetPlayerForwardDirection(context);
            float min = Mathf.Min(context.MinDistance, context.MaxDistance);
            float max = Mathf.Max(context.MinDistance, context.MaxDistance);

            for (int i = 0; i < 10; i++)
            {
                Vector2 direction = SpawnDirection.RandomDirectionInCone(forward, context.ForwardRespawnAngle);
                Vector2 pos = SpawnCameraBounds.TryGetSpawnRectBandPosition(context, direction, out Vector2 bandPosition)
                    ? bandPosition
                    : center + direction * Random.Range(min, max);

                if (context.IsInfiniteMap || !context.HasMapBounds || SpawnMapBounds.IsInMapBounds(context, pos, 0f))
                    return pos;
            }

            return SpawnMapBounds.ClampToMapBounds(context, RandomSpawnPosition(context), 0f);
        }

        public static Vector2 RandomItemChestSpawnPosition(SpawnerPositionContext context, float radius, float padding)
            => ItemChestSpawnPosition.RandomItemChestSpawnPosition(context, radius, padding);

        public static Vector2 RandomItemChestSpawnPosition(SpawnerPositionContext context, float minRadius, float maxRadius, float padding)
            => ItemChestSpawnPosition.RandomItemChestSpawnPosition(context, minRadius, maxRadius, padding);

        public static bool ShouldRecycleEnemy(SpawnerPositionContext context, Vector2 position)
            => SpawnCameraBounds.ShouldRecycleEnemy(context, position);

        public static Vector2 ClampToMapBounds(SpawnerPositionContext context, Vector2 pos, float padding)
            => SpawnMapBounds.ClampToMapBounds(context, pos, padding);

        public static bool IsInMapBounds(SpawnerPositionContext context, Vector2 pos, float padding)
            => SpawnMapBounds.IsInMapBounds(context, pos, padding);
    }
}
