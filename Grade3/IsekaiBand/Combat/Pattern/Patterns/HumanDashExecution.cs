using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanDashExecution
    {
        private readonly PianoBossHumanDashPatternSO _pattern;
        private static readonly int DashAnimatorParameter = Animator.StringToHash("DASH");
        public HumanDashExecution(PianoBossHumanDashPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || owner.IsDead)
            {
                _pattern.Preparation.RestorePreparedState();
                return;
            }

            if (_pattern.Preparation.Direction.sqrMagnitude <= Mathf.Epsilon
                && !HumanDashPreparation.TryGetTargetDirection(owner, out _pattern.Preparation.Direction))
            {
                _pattern.Preparation.RestorePreparedState();
                return;
            }

            Rigidbody2D body = owner.GetComponent<Rigidbody2D>();
            Collider2D bodyCollider = owner.GetComponent<Collider2D>();
            bool previousTrigger = bodyCollider != null && bodyCollider.isTrigger;
            bool previousKnockbackImmunity = owner.IsKnockbackImmune;
            var damagedTargets = new HashSet<IDamageable>();

            Vector2 start = owner.transform.position;
            Vector2 end = start + _pattern.Preparation.Direction * _pattern.DashDistance;
            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper != null)
                end = stageHelper.ClampToMapBound(end, _pattern.BoundaryPadding);

            try
            {
                owner.GetCompo<EntityRenderer>()?.FlipController(_pattern.Preparation.Direction.x);
                if (_pattern.Preparation.PreparedAnimator == null)
                    _pattern.Preparation.PreparedAnimator = owner.GetComponentInChildren<Animator>();
                _pattern.Preparation.PreparedAnimator?.SetBool(DashAnimatorParameter, true);

                owner.IsKnockbackImmune = true;
                if (bodyCollider != null)
                    bodyCollider.isTrigger = true;
                if (body != null)
                    body.linearVelocity = Vector2.zero;

                _pattern.PlaySound();
                _pattern.PlayVFX(_pattern.DashVFXItem, start, 1f, _pattern.Preparation.Direction);

                DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
                Vector2 previousPosition = start;
                float elapsed = 0f;

                while (elapsed < _pattern.DashDuration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                    if (owner == null || owner.IsDead)
                        return;

                    elapsed = Mathf.Min(_pattern.DashDuration, elapsed + Time.deltaTime);
                    float normalizedTime = Mathf.Clamp01(elapsed / _pattern.DashDuration);
                    float easedTime = HumanDashHits.EaseOutCubic(normalizedTime);
                    Vector2 nextPosition = Vector2.LerpUnclamped(start, end, easedTime);

                    _pattern.Hits.DamageBetween(owner, damage, previousPosition, nextPosition, damagedTargets);
                    HumanDashHits.MoveOwner(owner, body, nextPosition);
                    previousPosition = nextPosition;
                }

                HumanDashHits.MoveOwner(owner, body, end);
            }
            finally
            {
                if (bodyCollider != null)
                    bodyCollider.isTrigger = previousTrigger;
                if (body != null)
                    body.linearVelocity = Vector2.zero;
                if (owner != null)
                    owner.IsKnockbackImmune = previousKnockbackImmunity;

                _pattern.Preparation.RestorePreparedState();
            }
        }
    }
}
