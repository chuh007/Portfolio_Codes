using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepWarnings
    {
        private readonly Phase2LaneSweepPatternSO _pattern;
        private const int LaneCount = 3;
        public LaneSweepWarnings(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayWarnings(CancellationToken ct)
        {
            float duration = _pattern.WarningDuration / _pattern.SweepCount;
            for (int i = 0; i < _pattern.Plan.Sweeps.Count; i++)
            {
                await PlayWarning(_pattern.Plan.Sweeps[i], duration, ct);
                if (ct.IsCancellationRequested)
                    return;
            }
        }

        public async UniTask PlayWarning(LaneSweep sweep, float duration, CancellationToken ct)
        {
            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: ct).SuppressCancellationThrow();
                return;
            }

            var tasks = new List<UniTask>(LaneCount);
            for (int i = 0; i < sweep.LaneRects.Length; i++)
            {
                var poolable = _pattern.poolManager.Pop(_pattern.warningItem);
                if (poolable is not RectWarning warning)
                {
                    if (poolable != null)
                        _pattern.poolManager.Push(poolable);
                    continue;
                }

                Rect laneRect = sweep.LaneRects[i];
                warning.Setup(
                    laneRect.center,
                    Vector2.SignedAngle(Vector2.up, Vector2.left),
                    laneRect.height,
                    laneRect.width,
                    true,
                    RectWarningVisualMode.FillRect);
                tasks.Add(warning.PlayAsync(duration, ct));
            }

            if (tasks.Count == 0)
                await UniTask.WaitForSeconds(duration, cancellationToken: ct).SuppressCancellationThrow();
            else
                await UniTask.WhenAll(tasks);
        }
    }
}
