using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SkeletonMageFallWarning
    {
        private readonly SkeletonMageFallingSlowFieldPatternSO _pattern;

        public SkeletonMageFallWarning(SkeletonMageFallingSlowFieldPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayLandingWarningAsync(
            Vector2 landingPosition,
            float duration,
            CancellationToken ct)
        {
            if (_pattern.WarningDuration <= 0f)
                return;

            if (_pattern.ImpactRadius <= 0f || _pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is CircleWarning warning)
            {
                warning.Setup(landingPosition, _pattern.ImpactRadius);
                await warning.PlayAsync(duration, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(duration, cancellationToken: ct);
        }
    }
}
