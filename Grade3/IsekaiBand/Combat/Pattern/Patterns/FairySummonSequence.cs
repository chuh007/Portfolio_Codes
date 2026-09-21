using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FairySummonSequence
    {
        private readonly PianoBossFairySummonPatternSO _pattern;

        public FairySummonSequence(PianoBossFairySummonPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || _pattern.FairyMinionItem == null)
                return;

            int availableSlots = Mathf.Max(0, _pattern.MaxLivingMinions - _pattern.Minions.CountLivingMinions());
            int actualSummonCount = Mathf.Min(_pattern.SummonCount, availableSlots);
            if (actualSummonCount <= 0)
                return;

            _pattern.PlaySound();
            List<Vector2> spawnPositions = _pattern.Minions.BuildSpawnPositions(owner, actualSummonCount);
            await PlaySpawnWarnings(spawnPositions, ct);

            if (ct.IsCancellationRequested)
                return;

            Spawner spawner = Object.FindAnyObjectByType<Spawner>();
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                if (ct.IsCancellationRequested)
                    return;

                Enemy minion = spawner != null
                    ? spawner.SpawnSummonedAt(_pattern.FairyMinionItem, spawnPositions[i], owner.target)
                    : _pattern.Minions.SpawnByPool(owner, spawnPositions[i]);

                if (minion != null)
                    _pattern.PlayVFX(_pattern.SpawnVFXItem, minion.transform.position, 0.7f, Vector2.right);

                if (_pattern.SummonInterval > 0f && i < spawnPositions.Count - 1)
                    await UniTask.WaitForSeconds(_pattern.SummonInterval, cancellationToken: ct);
            }
        }

        public async UniTask PlaySpawnWarnings(IReadOnlyList<Vector2> positions, CancellationToken ct)
        {
            if (_pattern.SummonTelegraphDuration <= 0f)
                return;

            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(_pattern.SummonTelegraphDuration, cancellationToken: ct);
                return;
            }

            var warningTasks = new List<UniTask>(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                IPoolable poolable = _pattern.poolManager.Pop(_pattern.warningItem);
                if (poolable is CircleWarning warning)
                {
                    warning.Setup(positions[i], _pattern.SpawnWarningRadius);
                    warningTasks.Add(warning.PlayAsync(_pattern.SummonTelegraphDuration, ct));
                }
                else if (poolable != null)
                {
                    _pattern.poolManager.Push(poolable);
                }
            }

            if (warningTasks.Count > 0)
                await UniTask.WhenAll(warningTasks);
            else
                await UniTask.WaitForSeconds(_pattern.SummonTelegraphDuration, cancellationToken: ct);
        }
    }
}
