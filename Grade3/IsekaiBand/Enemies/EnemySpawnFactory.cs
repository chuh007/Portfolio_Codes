using System;
using System.Collections.Generic;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Enemies
{
    internal enum SpawnPositionMode { FindUnblocked, UseExact, FindOffscreen }

    internal class EnemySpawnFactory
    {
        private readonly Func<SpawnerSettings> _getSettings;
        private readonly List<Enemy> _enemies;
        private readonly SpawnerPositionTracker _positions;
        private readonly EnemySpawnHealth _health;
        private readonly SpawnCollisionQuery _collision = new SpawnCollisionQuery();
        public EnemySpawnSelection Selection { get; }

        public EnemySpawnFactory(Func<SpawnerSettings> getSettings, List<Enemy> enemies,
            SpawnerPositionTracker positions, EnemySpawnHealth health)
        {
            _getSettings = getSettings;
            _enemies = enemies;
            _positions = positions;
            _health = health;
            Selection = new EnemySpawnSelection(enemies, Spawn);
        }

        public void Initialize() => _collision.Initialize();

        public Enemy Spawn(
            PoolItemSO enemyItem,
            float healthMultiplier = 1f,
            bool applyHealthGrowth = true,
            float spawnDistanceMultiplier = 1f)
        {
            SpawnerPositionContext positionContext = _positions.CreatePositionContext();
            Vector2 spawnPosition = spawnDistanceMultiplier > 1f
                ? SpawnerPositionUtility.RandomDistantSpawnPosition(
                    positionContext,
                    spawnDistanceMultiplier)
                : SpawnerPositionUtility.RandomSpawnPosition(positionContext);

            return SpawnAt(
                enemyItem,
                spawnPosition,
                healthMultiplier,
                applyHealthGrowth,
                true,
                SpawnPositionMode.FindOffscreen);
        }

        public Enemy SpawnAt(PoolItemSO item, Vector2 position, float multiplier, bool applyGrowth)
            => SpawnAt(item, position, multiplier, applyGrowth, true);

        public Enemy SpawnAt(
            PoolItemSO enemyItem,
            Vector2 spawnPos,
            float healthMultiplier,
            bool applyHealthGrowth,
            bool applySpawnRestrictions,
            SpawnPositionMode positionMode = SpawnPositionMode.FindUnblocked)
        {
            if (_getSettings().poolManager == null || enemyItem == null) return null;
            if (applySpawnRestrictions && !Selection.CanSpawnEnemyItem(enemyItem)) return null;

            if (positionMode != SpawnPositionMode.UseExact
                && !_collision.TryGetUnblockedSpawnPosition(_positions.CreatePositionContext(), spawnPos, out spawnPos,
                    positionMode == SpawnPositionMode.FindOffscreen))
                return null;

            IPoolable poolable = _getSettings().poolManager.Pop(enemyItem, spawnPos, Quaternion.identity);
            if (poolable is not Enemy enemy)
            {
                Debug.LogWarning($"[Spawner] {enemyItem.name} prefab needs Enemy component.");
                if (poolable != null)
                    _getSettings().poolManager.Push(poolable);
                return null;
            }

            _enemies.Remove(enemy);
            _enemies.Add(enemy);
            float effectiveHealthMultiplier = _health.ResolveMultiplier(healthMultiplier, applyHealthGrowth);
            _health.ApplySpawnHealthScaling(
                enemy,
                effectiveHealthMultiplier,
                applyHealthGrowth);
            _health.ApplyOverallEnemyHealthMultiplier(enemy, _getSettings().overallEnemyHealthMultiplier);

            if (_positions.Player != null)
                enemy.SetTarget(_positions.Player);

            if (applySpawnRestrictions) Selection.RegisterSpawn(enemy, _getSettings().rangeEnemySpawnInterval);

            return enemy;
        }
    }
}
