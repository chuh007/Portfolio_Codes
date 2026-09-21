using System;
using System.Collections.Generic;
using _Work.CHUH.Code.WaveSystem;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemySpawnSelection
    {
        private const int MaxActiveRangeEnemyCount = 4;
        private const float EnemySpawnRateMultiplier = 1.1f;
        private readonly List<Enemy> _enemies;
        private readonly Func<PoolItemSO, float, bool, float, Enemy> _spawn;
        private float _nextRangeEnemySpawnTime;

        public EnemySpawnSelection(List<Enemy> enemies, Func<PoolItemSO, float, bool, float, Enemy> spawn)
        {
            _enemies = enemies;
            _spawn = spawn;
        }

        public void RegisterSpawn(Enemy enemy, float interval)
        {
            if (enemy is RangeEnemy)
                _nextRangeEnemySpawnTime = Time.time + Mathf.Max(0f, interval) / EnemySpawnRateMultiplier;
        }

        public bool TrySpawnRandomEnemy(
            List<WaveEnemyData> enemyPool,
            float spawnDistanceMultiplier = 1f)
        {
            if (enemyPool == null || enemyPool.Count == 0) return false;

            float totalWeight = 0f;
            foreach (var data in enemyPool)
            {
                if (data.wight > 0f && CanSpawnEnemyItem(data.enemyPrefab))
                    totalWeight += data.wight;
            }

            if (totalWeight <= 0) return false;

            float randomValue = Random.Range(0f, totalWeight);
            float weightSum = 0f;

            foreach (var data in enemyPool)
            {
                if (data.wight <= 0f || !CanSpawnEnemyItem(data.enemyPrefab))
                    continue;

                weightSum += data.wight;

                if (randomValue <= weightSum)
                {
                    return _spawn(
                        data.enemyPrefab,
                        data.multiplierValue,
                        true,
                        spawnDistanceMultiplier) != null;
                }
            }

            return false;
        }

        public bool CanSpawnEnemyItem(PoolItemSO enemyItem)
        {
            if (!IsRangeEnemyItem(enemyItem))
                return true;

            if (ActiveRangeEnemyCount >= MaxActiveRangeEnemyCount)
                return false;

            return Time.time >= _nextRangeEnemySpawnTime;
        }

        private int ActiveRangeEnemyCount
        {
            get
            {
                int count = 0;
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy != null && enemy is RangeEnemy && enemy.gameObject.activeInHierarchy && !enemy.IsDead)
                        count++;
                }

                return count;
            }
        }

        private static bool IsRangeEnemyItem(PoolItemSO enemyItem)
        {
            return enemyItem != null &&
                   enemyItem.prefab != null &&
                   enemyItem.prefab.TryGetComponent<RangeEnemy>(out _);
        }
    }
}
