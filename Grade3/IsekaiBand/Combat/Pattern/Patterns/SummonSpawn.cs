using _Work.CHUH.Code.Enemies;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SummonSpawn
    {
        private readonly SummonEnemiesPattern _pattern;

        public SummonSpawn(SummonEnemiesPattern pattern) => _pattern = pattern;

        public Enemy SpawnByPool(PoolItemSO enemyItem, Vector2 spawnPosition, Enemy owner)
        {
            if (_pattern.poolManager == null) return null;

            Vector2 clampedSpawnPosition = _pattern.Positions.ClampToMapBounds(spawnPosition);
            IPoolable poolable = _pattern.poolManager.Pop(
                enemyItem,
                clampedSpawnPosition,
                Quaternion.identity);
            if (poolable is not Enemy enemy)
            {
                Debug.LogWarning($"[SummonEnemiesPattern] {enemyItem.name} prefab needs Enemy component.");
                if (poolable != null)
                    _pattern.poolManager.Push(poolable);
                return null;
            }

            if (owner.target != null)
                enemy.SetTarget(owner.target);

            return enemy;
        }

        public void PlaySpawnVFX(Enemy spawnedEnemy)
        {
            if (spawnedEnemy == null) return;

            _pattern.PlayVFX(_pattern.SpawnVFXItem, spawnedEnemy.transform.position, 1f, Vector2.right);
        }
    }
}
