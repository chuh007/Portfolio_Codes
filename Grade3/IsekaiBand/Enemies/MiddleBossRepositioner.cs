using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using BossEnemy = _Work.CHUH.Code.Enemies.Boss.Boss;

namespace _Work.CHUH.Code.Enemies
{
    internal static class MiddleBossRepositioner
    {
        private const float MiddleBossFarViewMultiplier = 1.5f;
        private const float MiddleBossMinEdgeOffsetRatio = 0.05f;

        public static bool ShouldRepositionMiddleBoss(SpawnerPositionContext positionContext, Vector2 enemyPosition)
        {
            if (!TryGetCameraHalfSize(positionContext, out Vector2 halfSize))
                return false;

            Vector2 offset = enemyPosition - positionContext.CameraCenter;
            float xRatio = Mathf.Abs(offset.x) / halfSize.x;
            float yRatio = Mathf.Abs(offset.y) / halfSize.y;
            return Mathf.Max(xRatio, yRatio) >= MiddleBossFarViewMultiplier;
        }

        public static void TeleportMiddleBossNearViewEdge(
            Enemy enemy,
            Player player,
            SpawnerPositionContext positionContext)
        {
            if (!TryGetCameraHalfSize(positionContext, out Vector2 halfSize))
            {
                EnemyRepositioner.TeleportEnemyForward(enemy, player, positionContext);
                return;
            }

            Vector2 center = positionContext.CameraCenter;
            Vector2 direction = (Vector2)enemy.transform.position - center;
            if (direction.sqrMagnitude <= 0.0001f)
                direction = Vector2.right;

            direction.Normalize();

            float edgeDistance = DistanceToCameraRectEdge(halfSize, direction);
            if (float.IsNaN(edgeDistance) || float.IsInfinity(edgeDistance) || edgeDistance <= 0f)
                edgeDistance = Mathf.Max(halfSize.x, halfSize.y);

            float edgeOffsetRatio = Mathf.Max(MiddleBossMinEdgeOffsetRatio, positionContext.SpawnViewportMargin);
            Vector2 spawnPosition = center + direction * edgeDistance * (1f + edgeOffsetRatio);
            enemy.transform.position = SpawnerPositionUtility.ClampToMapBounds(positionContext, spawnPosition, 0f);

            if (enemy.TryGetComponent(out Rigidbody2D rb))
                rb.linearVelocity = Vector2.zero;

            enemy.SetTarget(player);
        }

        public static bool TryGetCameraHalfSize(SpawnerPositionContext positionContext, out Vector2 halfSize)
        {
            Camera camera = positionContext.MainCamera;
            if (camera == null || !camera.orthographic)
            {
                halfSize = default;
                return false;
            }

            float halfHeight = camera.orthographicSize;
            float halfWidth = halfHeight * camera.aspect;
            if (halfWidth <= 0f || halfHeight <= 0f)
            {
                halfSize = default;
                return false;
            }

            halfSize = new Vector2(halfWidth, halfHeight);
            return true;
        }

        public static float DistanceToCameraRectEdge(Vector2 halfSize, Vector2 direction)
        {
            float distanceX = float.PositiveInfinity;
            float distanceY = float.PositiveInfinity;

            if (Mathf.Abs(direction.x) > 0.0001f)
                distanceX = halfSize.x / Mathf.Abs(direction.x);

            if (Mathf.Abs(direction.y) > 0.0001f)
                distanceY = halfSize.y / Mathf.Abs(direction.y);

            return Mathf.Min(distanceX, distanceY);
        }

        public static bool IsMiddleBoss(Enemy enemy)
        {
            return enemy is MiddleBoss || enemy is BossEnemy { TreatAsMiddleBoss: true };
        }

        public static bool ShouldSkipReposition(Enemy enemy)
        {
            return IsMiddleBoss(enemy)
                   && enemy.GetCompo<EnemyAttackCompo>(true)?.IsSinglePatternRunning == true;
        }
    }
}
