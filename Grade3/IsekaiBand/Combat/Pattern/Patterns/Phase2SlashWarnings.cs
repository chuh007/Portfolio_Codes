using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashWarnings
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;

        public Phase2SlashWarnings(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayProjectileWarning(
            Enemy owner,
            Vector2 direction,
            float durationSeconds,
            CancellationToken ct)
        {
            if (durationSeconds <= 0f)
                return;

            float warningLength = _pattern.ProjectileWarningLength > 0f
                ? _pattern.ProjectileWarningLength
                : _pattern.ProjectileSpeed * _pattern.ProjectileLifeTime;
            Vector2 warningPosition = (Vector2)owner.transform.position + direction * (warningLength * 0.5f);
            float rotation = Vector2.SignedAngle(Vector2.up, direction);

            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(durationSeconds, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is RectWarning warning)
            {
                warning.Setup(
                    warningPosition,
                    rotation,
                    0f,
                    warningLength,
                    true,
                    RectWarningVisualMode.SingleLine);
                await warning.PlayAsync(durationSeconds, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(durationSeconds, cancellationToken: ct);
        }

        public async UniTask PlayDashWarning(Enemy owner, Vector2 direction, CancellationToken ct)
        {
            if (_pattern.DashWarningDuration <= 0f)
                return;

            float warningLength = _pattern.DashSpeed * _pattern.DashDuration;
            Vector2 warningPosition = (Vector2)owner.transform.position + direction * (warningLength * 0.5f);
            float rotation = Vector2.SignedAngle(Vector2.up, direction);

            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(_pattern.DashWarningDuration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is RectWarning warning)
            {
                warning.Setup(warningPosition, rotation, _pattern.DashWarningWidth, warningLength, true);
                await warning.PlayAsync(_pattern.DashWarningDuration, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(_pattern.DashWarningDuration, cancellationToken: ct);
        }
    }
}
