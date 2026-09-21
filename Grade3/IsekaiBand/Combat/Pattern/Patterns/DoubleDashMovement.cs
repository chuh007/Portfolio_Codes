using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DoubleDashMovement
    {
        private readonly DoubleDashPatternSO _pattern;
        private const float DashVFXInterval = 0.5f;
        private const float BoundaryDetectionPadding = 1.25f;
        private const float BoundaryStopPadding = 0.5f;
        public DoubleDashMovement(DoubleDashPatternSO pattern) => _pattern = pattern;

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

            render?.FlipController(direction.x);

            HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
            Vector2 previousPosition = owner.transform.position;

            _pattern.PlaySound();
            PlayDashVFX(owner, direction);
            float nextDashVFXTime = Time.time + DashVFXInterval;

            float startTime = Time.time;
            while (Time.time < startTime + _pattern.Duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
                if (ShouldStopDash(owner, rb, ct))
                    return;

                if (DashBoundaryUtility.TryGetStopPosition(
                        owner.transform.position,
                        direction,
                        BoundaryDetectionPadding,
                        BoundaryStopPadding,
                        out Vector2 stopPosition))
                {
                    _pattern.Targets.TryDamageTargets(owner, damage, previousPosition, stopPosition, direction, damagedTargets);
                    DashBoundaryUtility.MoveToStopPosition(owner, rb, stopPosition);
                    break;
                }

                if (Time.time >= nextDashVFXTime)
                {
                    PlayDashVFX(owner, direction);
                    nextDashVFXTime = Time.time + DashVFXInterval;
                }

                Vector2 currentPosition = owner.transform.position;
                _pattern.Targets.TryDamageTargets(owner, damage, previousPosition, currentPosition, direction, damagedTargets);
                previousPosition = currentPosition;

                float normalizedTime = _pattern.Duration > Mathf.Epsilon
                    ? Mathf.Clamp01((Time.time - startTime) / _pattern.Duration)
                    : 1f;
                rb.linearVelocity = direction * _pattern.Speed * _pattern.GetSpeedMultiplier(normalizedTime);
            }

            rb.linearVelocity = Vector2.zero;
        }

        public static bool ShouldStopDash(Enemy owner, Rigidbody2D rb, CancellationToken ct)
        {
            if (!ct.IsCancellationRequested && owner != null && !owner.IsDead)
                return false;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            return true;
        }

        public void PlayDashVFX(Enemy owner, Vector2 direction)
        {
            if (owner == null) return;

            _pattern.PlayVFX(_pattern.DashVFXItem, owner.transform.position, 1f, direction);
        }
    }
}
