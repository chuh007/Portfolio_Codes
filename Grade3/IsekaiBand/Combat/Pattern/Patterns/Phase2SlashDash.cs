using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashDash
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;
        private const float DashVFXInterval = 0.5f;
        private const float BoundaryDetectionPadding = 1.25f;
        private const float BoundaryStopPadding = 0.5f;
        public Phase2SlashDash(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public async UniTask DashAsync(
            Enemy owner,
            Rigidbody2D rb,
            EntityRenderer render,
            Vector2 direction,
            DamageData damage,
            CancellationToken ct)
        {
            if (rb == null || direction.sqrMagnitude <= Mathf.Epsilon)
                return;

            bool wasKnockbackImmune = owner.IsKnockbackImmune;
            owner.IsKnockbackImmune = true;

            try
            {
                render?.FlipController(direction.x);
                _pattern.PlaySound();
                rb.linearVelocity = direction * _pattern.DashSpeed;

                HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
                Vector2 previousPosition = owner.transform.position;
                PlayDashVFX(owner, direction);
                float nextDashVFXTime = Time.time + DashVFXInterval;
                float startTime = Time.time;

                while (Time.time < startTime + _pattern.DashDuration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                    if (DashBoundaryUtility.TryGetStopPosition(
                            owner.transform.position,
                            direction,
                            BoundaryDetectionPadding,
                            BoundaryStopPadding,
                            out Vector2 stopPosition))
                    {
                        _pattern.Targets.TryDamageTargets(owner, damage, previousPosition, stopPosition, direction, _pattern.DashHitRadius, damagedTargets);
                        DashBoundaryUtility.MoveToStopPosition(owner, rb, stopPosition);
                        break;
                    }

                    if (Time.time >= nextDashVFXTime)
                    {
                        PlayDashVFX(owner, direction);
                        nextDashVFXTime = Time.time + DashVFXInterval;
                    }

                    Vector2 currentPosition = owner.transform.position;
                    _pattern.Targets.TryDamageTargets(owner, damage, previousPosition, currentPosition, direction, _pattern.DashHitRadius, damagedTargets);
                    previousPosition = currentPosition;

                    rb.linearVelocity = direction * _pattern.DashSpeed;
                }
            }
            finally
            {
                owner.IsKnockbackImmune = wasKnockbackImmune;
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void PlayDashVFX(Enemy owner, Vector2 direction)
        {
            if (owner == null) return;

            _pattern.PlayVFX(_pattern.DashVFXItem, owner.transform.position, 1f, direction);
        }
    }
}
