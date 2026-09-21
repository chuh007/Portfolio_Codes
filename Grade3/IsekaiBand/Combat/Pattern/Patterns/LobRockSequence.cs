using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LobRockSequence
    {
        private readonly MiddleBossLobRockPatternSO _pattern;

        public LobRockSequence(MiddleBossLobRockPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            Vector2 launchPosition = (Vector2)owner.transform.position + Vector2.up * _pattern.LaunchHeight;
            Vector2[] landingPositions = _pattern.Landing.CreateLandingPositions(_pattern.Landing.GetLandingPosition(owner));
            DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
            damage.DamageType = _pattern.DamageType;

            UniTask[] tasks = new UniTask[landingPositions.Length + 1];
            tasks[0] = _pattern.Pose.PlayAttackPoseAsync(owner, ct);
            for (int i = 0; i < landingPositions.Length; i++)
            {
                float delay = _pattern.LaunchInterval * i;
                tasks[i + 1] = LaunchRockAsync(owner, launchPosition, landingPositions[i], damage, delay, ct);
            }

            await UniTask.WhenAll(tasks);
        }

        public async UniTask LaunchRockAsync(
            Enemy owner,
            Vector2 launchPosition,
            Vector2 landingPosition,
            DamageData damage,
            float delay,
            CancellationToken ct)
        {
            if (delay > 0f)
                await UniTask.WaitForSeconds(delay, cancellationToken: ct);

            if (ct.IsCancellationRequested)
                return;

            await UniTask.WhenAll(
                PlayLandingWarningAsync(landingPosition, ct),
                _pattern.Projectile.PlayRockArcAsync(launchPosition, landingPosition, ct));

            if (ct.IsCancellationRequested)
                return;

            _pattern.Impact.PlayImpactSound();
            _pattern.Impact.PlayImpactEffectAsync(landingPosition, ct).Forget();
            _pattern.Impact.DamageTargets(owner, landingPosition, damage);
        }

        public async UniTask PlayLandingWarningAsync(Vector2 landingPosition, CancellationToken ct)
        {
            if (_pattern.ImpactRadius <= 0f || _pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(_pattern.TravelDuration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is CircleWarning warning)
            {
                warning.Setup(landingPosition, _pattern.ImpactRadius);
                await warning.PlayAsync(_pattern.TravelDuration, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(_pattern.TravelDuration, cancellationToken: ct);
        }
    }
}
