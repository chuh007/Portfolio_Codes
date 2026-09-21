using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashFullAttack
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;

        public Phase2SlashFullAttack(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayFullAttackAndFire(
            Enemy owner,
            Vector2 direction,
            DamageData damage,
            bool reverse,
            CancellationToken ct)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, _pattern.FullAttackStateName, out int layerIndex, out int stateHash, _pattern.AnimatorLayer))
            {
                await _pattern.Warnings.PlayProjectileWarning(owner, direction, _pattern.ProjectileWarningDuration, ct);
                _pattern.Projectiles.SpawnSlashProjectile(owner, direction, damage, ct);
                return;
            }

            float animationDuration = _pattern.Animation.GetFullAttackDuration();
            float triggerNormalizedTime = _pattern.Animation.GetFullAttackTriggerNormalizedTime(animationDuration);
            float warningDuration = Mathf.Max(
                _pattern.ProjectileWarningDuration,
                Phase2SlashTimeline.GetTimeUntilTrigger(animationDuration, triggerNormalizedTime, reverse));
            bool didFire = false;
            EntityAnimatorTrigger animatorTrigger = owner.GetCompo<EntityAnimatorTrigger>();

            void FireOnce()
            {
                if (didFire || Phase2SlashTargets.ShouldStop(owner, ct))
                    return;

                didFire = true;
                _pattern.Projectiles.SpawnSlashProjectile(owner, direction, damage, ct);
            }

            if (animatorTrigger != null)
                animatorTrigger.OnAnimationTrigger += FireOnce;

            try
            {
                PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, true);
                PatternAnimatorUtility.SetAttackBlendValue(animator, _pattern.BlendTreeValue, _pattern.AttackBlendParameter, _pattern.FallbackBlendParameter);

                await UniTask.WhenAll(
                    _pattern.Warnings.PlayProjectileWarning(owner, direction, warningDuration, ct),
                    _pattern.Timeline.PlayAnimatorStateWithTrigger(
                        animator,
                        layerIndex,
                        stateHash,
                        animationDuration,
                        triggerNormalizedTime,
                        reverse,
                        FireOnce,
                        ct));

                FireOnce();
            }
            finally
            {
                if (animatorTrigger != null)
                    animatorTrigger.OnAnimationTrigger -= FireOnce;

                PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, false);
                _pattern.Animation.PlayIdleAnimation(animator, layerIndex);
            }
        }
    }
}
