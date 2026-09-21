using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    internal struct WaveFillSettings
    {
        public float Duration;
        public int BatchSize;
        public float RecoveryDuration;
        public float RecoveryDistance;
    }

    internal class WaveMinimumFill : IDisposable
    {
        private readonly WaveSpawnController _spawning;
        private CancellationTokenSource _fillCts;
        public bool IsFilling => _fillCts != null;

        public WaveMinimumFill(WaveSpawnController spawning) => _spawning = spawning;

        public void Begin(float duration, float distance, int batchSize)
        {
            Dispose();
            _fillCts = new CancellationTokenSource();
            FillAsync(duration, distance, batchSize, _fillCts.Token).Forget();
        }

        public void Dispose()
        {
            _fillCts?.Cancel();
            _fillCts?.Dispose();
            _fillCts = null;
        }

        private async UniTaskVoid FillAsync(float duration, float distance, int batchSize, CancellationToken ct)
        {
            try
            {
                int deficit = Mathf.Max(0, _spawning.MinimumCount - _spawning.ActiveCount);
                if (deficit <= 0) return;
                var budget = new WaveFillBudget(deficit, duration, batchSize);
                while (_spawning.CanSpawn && _spawning.ActiveCount < _spawning.MinimumCount)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                    if (Time.timeScale <= 0f) continue;
                    if (!_spawning.CanSpawn) break;

                    int target = Mathf.Min(_spawning.MinimumCount, _spawning.MaximumCount);
                    int missing = target - _spawning.ActiveCount;
                    if (missing <= 0) break;
                    int attempts = budget.GetSpawnAttempts(missing, Time.deltaTime);
                    if (attempts <= 0) continue;

                    int spawned = SpawnBatch(attempts, distance);
                    budget.Consume(spawned);
                    if (spawned > 0) continue;
                    await UniTask.Delay(TimeSpan.FromSeconds(_spawning.SpawnInterval),
                        DelayType.DeltaTime, PlayerLoopTiming.Update, ct);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_fillCts != null && _fillCts.Token == ct)
                {
                    _fillCts.Dispose();
                    _fillCts = null;
                }
            }
        }

        private int SpawnBatch(int attempts, float distance)
        {
            int spawned = 0;
            for (int i = 0; i < attempts; i++)
            {
                if (!_spawning.TrySpawn(distance)) break;
                spawned++;
            }
            return spawned;
        }
    }
}
