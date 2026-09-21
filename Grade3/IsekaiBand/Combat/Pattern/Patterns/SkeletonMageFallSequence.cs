using System.Threading;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SkeletonMageFallSequence
    {
        private readonly SkeletonMageFallingSlowFieldPatternSO _pattern;

        public SkeletonMageFallSequence(SkeletonMageFallingSlowFieldPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (!SkeletonMageFallingSlowFieldContext.TryConsumePreparedLandingPosition(owner, out Vector2 landingPosition))
                landingPosition = SkeletonMageFallingSlowFieldContext.GetLandingPosition(owner);

            EntityMover mover = owner != null ? owner.GetCompo<EntityMover>() : null;
            bool previousCanManualMove = mover != null && mover.CanManualMove;
            float timeToImpact = _pattern.GetTelegraphDuration(_pattern.FallDuration);

            mover?.StopImmediately();
            if (mover != null)
                mover.CanManualMove = false;

            try
            {
                await UniTask.WhenAll(
                    _pattern.Warning.PlayLandingWarningAsync(landingPosition, timeToImpact, ct),
                    _pattern.Projectile.PlayProjectileFallAsync(landingPosition, timeToImpact, ct));

                if (!ct.IsCancellationRequested)
                    _pattern.Impact.ApplyLandingImpact(owner, landingPosition);
            }
            finally
            {
                if (mover != null)
                {
                    mover.StopImmediately();
                    mover.CanManualMove = previousCanManualMove;
                }
            }
        }
    }
}
