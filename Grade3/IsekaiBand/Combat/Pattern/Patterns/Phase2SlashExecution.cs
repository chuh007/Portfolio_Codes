using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashExecution
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;

        public Phase2SlashExecution(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
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

                if (_pattern.InitialDelay > 0f)
                    await UniTask.WaitForSeconds(_pattern.InitialDelay, cancellationToken: ct);

                DamageData projectileDamage = _pattern.GetDamage(owner, _pattern.ProjectileDamageMultiplier);
                int count = Mathf.Max(1, _pattern.ProjectileCount);
                for (int i = 0; i < count; i++)
                {
                    if (Phase2SlashTargets.ShouldStop(owner, ct))
                        return;

                    float shotStartTime = Time.time;
                    Vector2 direction = Phase2SlashTargets.GetTargetDirection(owner);
                    render?.FlipController(direction.x);

                    await _pattern.FullAttack.PlayFullAttackAndFire(owner, direction, projectileDamage, i == 1, ct);

                    if (i >= count - 1)
                        continue;

                    float remainingInterval = _pattern.FireInterval - (Time.time - shotStartTime);
                    if (remainingInterval > 0f)
                        await UniTask.WaitForSeconds(remainingInterval, cancellationToken: ct);
                }

                if (_pattern.DashDelay > 0f)
                    await UniTask.WaitForSeconds(_pattern.DashDelay, cancellationToken: ct);

                if (Phase2SlashTargets.ShouldStop(owner, ct))
                    return;

                Vector2 dashDirection = Phase2SlashTargets.GetTargetDirection(owner);
                render?.FlipController(dashDirection.x);
                await _pattern.Warnings.PlayDashWarning(owner, dashDirection, ct);

                if (Phase2SlashTargets.ShouldStop(owner, ct))
                    return;

                DamageData dashDamage = _pattern.GetDamage(owner, _pattern.DashDamageMultiplier);
                _pattern.Animation.PlayDashAttackAnimation(owner);
                await _pattern.Dash.DashAsync(owner, rb, render, dashDirection, dashDamage, ct);
            }
            finally
            {
                if (bodyCollider != null)
                    bodyCollider.isTrigger = wasTrigger;

                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                if (mover != null)
                    mover.CanManualMove = previousCanManualMove;

                owner.IsKnockbackImmune = wasKnockbackImmune;
                _pattern.Animation.ResetAttackAnimation(owner);
            }
        }
    }
}
