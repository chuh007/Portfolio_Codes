using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class SpawnCameraBounds
    {
        public static Vector2 ClampInsideDespawnBounds(
            SpawnerPositionContext context,
            Vector2 position)
        {
            if (!context.HasCameraSize)
                return position;

            Vector2 safeHalfSize = Vector2.Lerp(
                context.SpawnMaxHalfSize,
                context.DespawnHalfSize,
                0.5f);
            Vector2 offset = position - context.CameraCenter;
            float scale = 1f;

            if (Mathf.Abs(offset.x) > safeHalfSize.x)
                scale = Mathf.Min(scale, safeHalfSize.x / Mathf.Abs(offset.x));
            if (Mathf.Abs(offset.y) > safeHalfSize.y)
                scale = Mathf.Min(scale, safeHalfSize.y / Mathf.Abs(offset.y));

            return context.CameraCenter + offset * scale;
        }

        public static bool TryGetSpawnRectBandPosition(SpawnerPositionContext context, Vector2 direction, out Vector2 position)
        {
            position = default;
            if (!context.HasCameraSize)
                return false;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = SpawnDirection.RandomDirection();

            direction.Normalize();

            float innerDistance = DistanceToCameraRectEdge(context, context.SpawnMinHalfSize, context.CameraCenter, direction);
            float outerDistance = DistanceToCameraRectEdge(context, context.SpawnMaxHalfSize, context.CameraCenter, direction);
            if (float.IsNaN(innerDistance) || float.IsInfinity(innerDistance) ||
                float.IsNaN(outerDistance) || float.IsInfinity(outerDistance) ||
                innerDistance <= 0f || outerDistance <= innerDistance)
            {
                return false;
            }

            float distance = Random.Range(innerDistance, outerDistance);
            position = context.CameraCenter + direction * distance;
            return true;
        }

        public static float DistanceToCameraRectEdge(
            SpawnerPositionContext context,
            Vector2 halfSize,
            Vector2 origin,
            Vector2 direction)
        {
            Vector2 rectCenter = context.CameraCenter;
            float distanceX = float.PositiveInfinity;
            float distanceY = float.PositiveInfinity;

            if (Mathf.Abs(direction.x) > 0.0001f)
            {
                float edgeX = rectCenter.x + (direction.x > 0f ? halfSize.x : -halfSize.x);
                distanceX = (edgeX - origin.x) / direction.x;
            }

            if (Mathf.Abs(direction.y) > 0.0001f)
            {
                float edgeY = rectCenter.y + (direction.y > 0f ? halfSize.y : -halfSize.y);
                distanceY = (edgeY - origin.y) / direction.y;
            }

            return Mathf.Min(distanceX, distanceY);
        }

        public static bool ContainsCameraRect(SpawnerPositionContext context, Vector2 halfSize, Vector2 position)
        {
            Vector2 center = context.CameraCenter;
            return position.x >= center.x - halfSize.x && position.x <= center.x + halfSize.x
                   && position.y >= center.y - halfSize.y && position.y <= center.y + halfSize.y;
        }

        public static bool ShouldRecycleEnemy(SpawnerPositionContext context, Vector2 position)
        {
            return context.HasCameraSize && !ContainsCameraRect(context, context.DespawnHalfSize, position);
        }

        public static bool IsOutsideSpawnBounds(SpawnerPositionContext context, Vector2 position)
        {
            return context.HasCameraSize && !ContainsCameraRect(context, context.SpawnMinHalfSize, position);
        }
    }
}
