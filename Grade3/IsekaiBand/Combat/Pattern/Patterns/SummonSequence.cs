using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SummonSequence
    {
        private readonly SummonEnemiesPattern _pattern;

        public SummonSequence(SummonEnemiesPattern pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || _pattern.EnemyItems == null || _pattern.EnemyItems.Count == 0)
                return;

            List<SummonInfo> summons = _pattern.Positions.CreateSummonInfos(owner.transform.position);
            if (summons.Count == 0)
                return;

            _pattern.PlaySound();

            if (_pattern.SummonDelay > 0f)
                await PlaySpawnWarnings(summons, _pattern.SummonDelay, ct);

            if (ct.IsCancellationRequested) return;

            Spawner spawner = Object.FindAnyObjectByType<Spawner>();

            for (int i = 0; i < summons.Count; i++)
            {
                if (ct.IsCancellationRequested) return;

                SummonInfo summon = summons[i];
                Enemy spawnedEnemy = spawner != null
                    ? spawner.SpawnAt(summon.EnemyItem, summon.Position)
                    : _pattern.Spawn.SpawnByPool(summon.EnemyItem, summon.Position, owner);

                _pattern.Spawn.PlaySpawnVFX(spawnedEnemy);

                if (_pattern.SummonInterval > 0f && i < summons.Count - 1)
                    await UniTask.WaitForSeconds(_pattern.SummonInterval, cancellationToken: ct);
            }
        }

        public async UniTask PlaySpawnWarnings(IReadOnlyList<SummonInfo> summons, float duration, CancellationToken ct)
        {
            if (_pattern.warningItem == null || _pattern.poolManager == null)
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);
                return;
            }

            var warningTasks = new List<UniTask>(summons.Count);
            for (int i = 0; i < summons.Count; i++)
            {
                IPoolable poolable = _pattern.poolManager.Pop(_pattern.warningItem);
                if (poolable is CircleWarning warning)
                {
                    warning.Setup(summons[i].Position, _pattern.SpawnWarningRadius);
                    warningTasks.Add(warning.PlayAsync(duration, ct));
                    continue;
                }

                if (poolable != null)
                    _pattern.poolManager.Push(poolable);
            }

            if (warningTasks.Count == 0)
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);
                return;
            }

            await UniTask.WhenAll(warningTasks);
        }
    }
}
