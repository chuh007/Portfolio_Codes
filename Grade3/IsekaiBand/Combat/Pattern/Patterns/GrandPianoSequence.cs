using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GrandPianoSequence
    {
        private readonly PianoBossHumanGrandPianoDropPatternSO _pattern;

        public GrandPianoSequence(PianoBossHumanGrandPianoDropPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            if (!GrandPianoLanding.TryConsumeLandingPosition(owner, out Vector2 landingPosition))
                landingPosition = _pattern.Landing.ResolveLandingPosition(owner);

            DamageData damage = _pattern.GetNoteDamage(owner, _pattern.DamageMultiplier);
            float timeToImpact = _pattern.GetTelegraphDuration(_pattern.FallDuration);
            await UniTask.WhenAll(
                PlayWarningAsync(landingPosition, timeToImpact, ct),
                DropGrandPianoAsync(owner, landingPosition, damage, timeToImpact, ct));
        }

        public async UniTask PlayWarningAsync(
            Vector2 landingPosition,
            float duration,
            CancellationToken ct)
        {
            if (_pattern.WarningDuration <= 0f)
                return;

            if (_pattern.poolManager == null || _pattern.warningItem == null)
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

        public async UniTask DropGrandPianoAsync(
            Enemy owner,
            Vector2 landingPosition,
            DamageData damage,
            float duration,
            CancellationToken ct)
        {
            Vector2 startPosition = landingPosition + Vector2.up * _pattern.FallHeight;
            GameObject piano = _pattern.Visual.CreateGrandPiano(startPosition);

            try
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    float normalizedTime = Mathf.Clamp01(elapsed / duration);
                    _pattern.Visual.SampleFall(piano, startPosition, landingPosition, normalizedTime);
                    elapsed += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                _pattern.Visual.SampleFall(piano, startPosition, landingPosition, 1f);
                if (ct.IsCancellationRequested)
                    return;

                _pattern.Impact.PlayImpactAudio(owner);
                _pattern.Impact.PlayImpactShockwave(landingPosition);
                CameraShakeUtility.PlayImpact(
                    landingPosition,
                    _pattern.CameraShakeStrength,
                    _pattern.CameraShakeDuration);
                _pattern.Impact.DamageTargets(owner, landingPosition, damage);

                if (_pattern.ImpactHoldDuration > 0f)
                    await UniTask.WaitForSeconds(_pattern.ImpactHoldDuration, cancellationToken: ct);

                await _pattern.Visual.FadeOutAsync(piano, ct);
            }
            finally
            {
                if (piano != null)
                    ProjectilePool.Push(piano);
            }
        }
    }
}
