using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemySpawnCleanup
    {
        private readonly MonoBehaviour _owner;
        private readonly List<Enemy> _enemies;
        private readonly Func<float> _getBudget;
        private readonly Func<int> _getMaxEnemiesPerFrame;

        public EnemySpawnCleanup(MonoBehaviour owner, List<Enemy> enemies,
            Func<float> getBudget, Func<int> getMaxEnemiesPerFrame)
        {
            _owner = owner;
            _enemies = enemies;
            _getBudget = getBudget;
            _getMaxEnemiesPerFrame = getMaxEnemiesPerFrame;
        }

        public void ClearEnemiesExcept(Enemy excludedEnemy, bool playDeathAnimation = false)
        {
            var enemiesToClear = new List<(Enemy enemy, int lifecycleVersion)>(_enemies.Count);
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = _enemies[i];
                if (enemy == null)
                {
                    _enemies.RemoveAt(i);
                    continue;
                }

                if (enemy == excludedEnemy)
                    continue;

                _enemies.RemoveAt(i);
                if (!enemy.gameObject.activeInHierarchy)
                    continue;

                enemiesToClear.Add((enemy, enemy.PoolLifecycleVersion));
            }

            if (enemiesToClear.Count > 0)
                _owner.StartCoroutine(ClearEnemiesWithinFrameBudget(enemiesToClear, playDeathAnimation));
        }

        private IEnumerator ClearEnemiesWithinFrameBudget(
            IReadOnlyList<(Enemy enemy, int lifecycleVersion)> enemiesToClear,
            bool playDeathAnimation)
        {
            double frameBudgetSeconds = Mathf.Max(0.1f, _getBudget()) * 0.001d;
            int maxEnemiesPerFrame = Mathf.Max(1, _getMaxEnemiesPerFrame());
            int processedThisFrame = 0;
            double frameDeadline = Time.realtimeSinceStartupAsDouble + frameBudgetSeconds;

            for (int i = 0; i < enemiesToClear.Count; i++)
            {
                var (enemy, lifecycleVersion) = enemiesToClear[i];
                if (enemy != null && enemy.gameObject.activeInHierarchy
                    && enemy.IsCurrentPoolLifecycle(lifecycleVersion))
                {
                    if (playDeathAnimation && enemy is FSMEnemy)
                        enemy.PlayDespawnDeathAnimation();
                    else
                        enemy.ReturnToPool();
                }

                processedThisFrame++;
                if (i >= enemiesToClear.Count - 1
                    || (processedThisFrame < maxEnemiesPerFrame
                        && Time.realtimeSinceStartupAsDouble < frameDeadline))
                    continue;

                yield return null;
                processedThisFrame = 0;
                frameDeadline = Time.realtimeSinceStartupAsDouble + frameBudgetSeconds;
            }
        }
    }
}
