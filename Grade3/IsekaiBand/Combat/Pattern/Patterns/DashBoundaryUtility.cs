using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal static class DashBoundaryUtility
    {
        private const float BoundsEpsilon = 0.001f;

        public static bool TryGetStopPosition(
            Vector2 position,
            Vector2 direction,
            float detectionPadding,
            float stopPadding,
            out Vector2 stopPosition)
        {
            stopPosition = position;

            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper == null || stageHelper.CheckMapBound(position, detectionPadding))
                return false;

            Vector2 detectionClampedPosition = stageHelper.ClampToMapBound(position, detectionPadding);
            Vector2 outsideDirection = position - detectionClampedPosition;
            if (outsideDirection.sqrMagnitude <= Mathf.Epsilon || Vector2.Dot(outsideDirection, direction) <= 0f)
                return false;

            stopPosition = GetForwardStopPosition(stageHelper, position, direction, stopPadding);
            return true;
        }

        public static void MoveToStopPosition(Enemy owner, Rigidbody2D rb, Vector2 stopPosition)
        {
            if (owner == null)
                return;

            if (rb != null)
                rb.position = stopPosition;

            Vector3 currentPosition = owner.transform.position;
            owner.transform.position = new Vector3(stopPosition.x, stopPosition.y, currentPosition.z);
        }

        private static Vector2 GetForwardStopPosition(
            StageHelper stageHelper,
            Vector2 position,
            Vector2 direction,
            float padding)
        {
            if (!stageHelper.IsBossArenaActive || direction.sqrMagnitude <= Mathf.Epsilon)
                return stageHelper.ClampToMapBound(position, padding);

            Rect bounds = GetPaddedBossArenaBounds(stageHelper, padding);
            if (!Contains(bounds, position))
                return stageHelper.ClampToMapBound(position, padding);

            Vector2 normalizedDirection = direction.normalized;
            float bestDistance = float.PositiveInfinity;

            TryUseVerticalSide(bounds.xMin, bounds, position, normalizedDirection, ref bestDistance);
            TryUseVerticalSide(bounds.xMax, bounds, position, normalizedDirection, ref bestDistance);
            TryUseHorizontalSide(bounds.yMin, bounds, position, normalizedDirection, ref bestDistance);
            TryUseHorizontalSide(bounds.yMax, bounds, position, normalizedDirection, ref bestDistance);

            if (float.IsPositiveInfinity(bestDistance))
                return stageHelper.ClampToMapBound(position, padding);

            return position + normalizedDirection * bestDistance;
        }

        private static Rect GetPaddedBossArenaBounds(StageHelper stageHelper, float padding)
        {
            Vector2 center = stageHelper.BossArenaCenter;
            Vector2 halfSize = stageHelper.BossArenaSize * 0.5f;
            float halfWidth = Mathf.Max(0f, halfSize.x - padding);
            float halfHeight = Mathf.Max(0f, halfSize.y - padding);

            return Rect.MinMaxRect(
                center.x - halfWidth,
                center.y - halfHeight,
                center.x + halfWidth,
                center.y + halfHeight);
        }

        private static void TryUseVerticalSide(
            float sideX,
            Rect bounds,
            Vector2 position,
            Vector2 direction,
            ref float bestDistance)
        {
            if (Mathf.Abs(direction.x) <= Mathf.Epsilon)
                return;

            float distance = (sideX - position.x) / direction.x;
            if (distance < 0f || distance >= bestDistance)
                return;

            float y = position.y + direction.y * distance;
            if (y < bounds.yMin - BoundsEpsilon || y > bounds.yMax + BoundsEpsilon)
                return;

            bestDistance = distance;
        }

        private static void TryUseHorizontalSide(
            float sideY,
            Rect bounds,
            Vector2 position,
            Vector2 direction,
            ref float bestDistance)
        {
            if (Mathf.Abs(direction.y) <= Mathf.Epsilon)
                return;

            float distance = (sideY - position.y) / direction.y;
            if (distance < 0f || distance >= bestDistance)
                return;

            float x = position.x + direction.x * distance;
            if (x < bounds.xMin - BoundsEpsilon || x > bounds.xMax + BoundsEpsilon)
                return;

            bestDistance = distance;
        }

        private static bool Contains(Rect bounds, Vector2 position)
        {
            return position.x >= bounds.xMin - BoundsEpsilon
                   && position.x <= bounds.xMax + BoundsEpsilon
                   && position.y >= bounds.yMin - BoundsEpsilon
                   && position.y <= bounds.yMax + BoundsEpsilon;
        }
    }
}
