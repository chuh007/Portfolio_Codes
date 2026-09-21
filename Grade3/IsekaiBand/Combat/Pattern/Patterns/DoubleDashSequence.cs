using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DoubleDashSequence
    {
        private readonly DoubleDashPatternSO _pattern;
        private Vector2 _preparedDirection;
        public DoubleDashSequence(DoubleDashPatternSO pattern) => _pattern = pattern;

        public async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>().StopImmediately();
            _preparedDirection = DoubleDashTargets.GetTargetDirection(owner);
            await _pattern.Warnings.PlayWarning(owner, _preparedDirection, _pattern.WarningDuration, ct);
        }

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || owner.IsDead)
                return;

            EntityRenderer render = owner.GetCompo<EntityRenderer>();
            EntityMover mover = owner.GetCompo<EntityMover>();
            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            Collider2D bodyCollider = owner.GetComponent<Collider2D>();

            bool previousCanManualMove = mover != null && mover.CanManualMove;
            bool wasKnockbackImmune = owner.IsKnockbackImmune;
            bool wasTrigger = false;
            if (bodyCollider != null)
            {
                wasTrigger = bodyCollider.isTrigger;
                bodyCollider.isTrigger = true;
            }

            try
            {
                owner.IsKnockbackImmune = true;

                if (mover != null)
                    mover.CanManualMove = false;

                DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);

                Vector2 firstDirection = _preparedDirection.sqrMagnitude > Mathf.Epsilon
                    ? _preparedDirection
                    : DoubleDashTargets.GetTargetDirection(owner);

                _pattern.Animation.PlayAttackAnimation(owner);
                await _pattern.Movement.DashAsync(owner, rb, render, firstDirection, damage, ct);
                if (_pattern.UseWarningAnimation)
                    _pattern.Animation.ResetAttackAnimation(owner);
                if (DoubleDashMovement.ShouldStopDash(owner, rb, ct))
                    return;

                if (_pattern.DashInterval > 0f)
                    await UniTask.WaitForSeconds(_pattern.DashInterval, cancellationToken: ct);
                if (DoubleDashMovement.ShouldStopDash(owner, rb, ct))
                    return;

                Vector2 secondDirection = DoubleDashTargets.GetTargetDirection(owner);
                await _pattern.Warnings.PlayWarning(owner, secondDirection, _pattern.SecondWarningDuration, ct);
                if (DoubleDashMovement.ShouldStopDash(owner, rb, ct))
                    return;

                _pattern.Animation.PlayAttackAnimation(owner);
                await _pattern.Movement.DashAsync(owner, rb, render, secondDirection, damage, ct);
                if (_pattern.UseWarningAnimation)
                    _pattern.Animation.ResetAttackAnimation(owner);
            }
            finally
            {
                if (bodyCollider != null)
                    bodyCollider.isTrigger = wasTrigger;

                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                if (mover != null)
                {
                    if (owner != null && owner.IsDead)
                    {
                        mover.StopImmediately();
                        mover.CanManualMove = false;
                    }
                    else
                    {
                        mover.CanManualMove = previousCanManualMove;
                    }
                }

                owner.IsKnockbackImmune = wasKnockbackImmune;

                if (render != null && owner != null && !owner.IsDead)
                    render.FlipController(DoubleDashTargets.GetTargetDirection(owner).x);

                _pattern.Animation.ResetAttackAnimation(owner);
            }
        }
    }
}
