using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FallingNotesSequence
    {
        private readonly PianoBossHumanFallingNotesPatternSO _pattern;

        public FallingNotesSequence(PianoBossHumanFallingNotesPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (!FallingNotesLanding.TryConsumeLandingPositions(owner, out Vector2[] landingPositions))
                landingPositions = _pattern.Landing.CreateLandingPositions(owner);

            DamageData damage = _pattern.GetNoteDamage(owner, _pattern.DamageMultiplier);
            float timeToImpact = _pattern.GetTelegraphDuration(_pattern.FallDuration);
            var tasks = new UniTask[landingPositions.Length];
            for (int i = 0; i < landingPositions.Length; i++)
            {
                tasks[i] = DropWarnedNoteAsync(
                    owner,
                    landingPositions[i],
                    _pattern.NoteSprite(i),
                    damage,
                    timeToImpact,
                    _pattern.FallStaggerInterval * i,
                    ct);
            }

            await UniTask.WhenAll(tasks);
        }

        public async UniTask PlayWarningAsync(
            Vector2 position,
            float duration,
            CancellationToken ct)
        {
            if (_pattern.WarningDuration <= 0f)
                return;

            if (_pattern.poolManager == null || _pattern.warningItem == null || _pattern.ImpactRadius <= 0f)
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is CircleWarning warning)
            {
                warning.Setup(position, _pattern.ImpactRadius);
                await warning.PlayAsync(duration, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(duration, cancellationToken: ct);
        }

        public async UniTask DropWarnedNoteAsync(
            Enemy owner,
            Vector2 landingPosition,
            Sprite sprite,
            DamageData damage,
            float timeToImpact,
            float delay,
            CancellationToken ct)
        {
            if (delay > 0f)
                await UniTask.WaitForSeconds(delay, cancellationToken: ct);

            await UniTask.WhenAll(
                PlayWarningAsync(landingPosition, timeToImpact, ct),
                _pattern.Visual.PlayNoteFallAsync(landingPosition, sprite, timeToImpact, ct));

            if (ct.IsCancellationRequested)
                return;

            _pattern.Impact.PlayImpactShockwave(landingPosition);
            _pattern.Impact.DamageTargets(owner, landingPosition, damage);
        }
    }
}
