using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using BossEnemy = _Work.CHUH.Code.Enemies.Boss.Boss;

namespace _Work.CHUH.Code.Enemies
{
    internal static class EnemyRepositioner
    {
        public static void RepositionFarEnemies(
            List<Enemy> enemies,
            Player player,
            PoolManagerSO poolManager,
            SpawnerPositionContext positionContext,
            Action<PoolItemSO, Vector2> spawnAt)
        {
            if (player == null) return;

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                if (!enemy.gameObject.activeInHierarchy)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                if (enemy.IsDead)
                    continue;

                if (enemy.IsAutomaticRepositionSuppressed)
                    continue;

                if (enemy is BossEnemy { TreatAsMiddleBoss: false })
                    continue;

                if (MiddleBossRepositioner.ShouldSkipReposition(enemy))
                    continue;

                Vector2 enemyPosition = enemy.transform.position;
                if (MiddleBossRepositioner.IsMiddleBoss(enemy))
                {
                    if (MiddleBossRepositioner.ShouldRepositionMiddleBoss(positionContext, enemyPosition))
                        MiddleBossRepositioner.TeleportMiddleBossNearViewEdge(enemy, player, positionContext);

                    continue;
                }

                if (!SpawnerPositionUtility.ShouldRecycleEnemy(positionContext, enemyPosition))
                    continue;

                DespawnAndRespawnEnemy(enemy, i, enemies, poolManager, positionContext, spawnAt);
            }
        }

        private static void DespawnAndRespawnEnemy(
            Enemy enemy,
            int index,
            List<Enemy> enemies,
            PoolManagerSO poolManager,
            SpawnerPositionContext positionContext,
            Action<PoolItemSO, Vector2> spawnAt)
        {
            PoolItemSO poolItem = enemy.PoolItem;
            if (poolManager == null || poolItem == null)
            {
                TeleportEnemyForward(enemy, positionContext.Player, positionContext);
                return;
            }

            Vector2 spawnPosition = SpawnerPositionUtility.RandomForwardSpawnPosition(positionContext);

            enemies.RemoveAt(index);
            if (enemy.gameObject.activeInHierarchy)
                enemy.ReturnToPool();

            spawnAt?.Invoke(poolItem, spawnPosition);
        }

        internal static void TeleportEnemyForward(Enemy enemy, Player player, SpawnerPositionContext positionContext)
        {
            Vector2 spawnPosition = SpawnerPositionUtility.RandomForwardSpawnPosition(positionContext);
            spawnPosition = SpawnerPositionUtility.ClampToMapBounds(positionContext, spawnPosition, 0f);
            if (!SpawnCameraBounds.IsOutsideSpawnBounds(positionContext, spawnPosition)) return;

            enemy.transform.position = spawnPosition;
            if (enemy.TryGetComponent(out Rigidbody2D rb))
                rb.linearVelocity = Vector2.zero;

            enemy.SetTarget(player);
        }
    }
}
