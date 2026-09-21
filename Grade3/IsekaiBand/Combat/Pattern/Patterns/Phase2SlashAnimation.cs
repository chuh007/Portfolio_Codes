using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashAnimation
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;
        private const float MinAnimationDuration = 0.01f;
        private const float FallbackFullAttackDuration = 0.58f;
        private const float FallbackFullAttackTriggerNormalizedTime = 0.57f;
        private const string AnimationTriggerFunctionName = "AnimationTrigger";
        public Phase2SlashAnimation(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public void PlayDashAttackAnimation(Enemy owner)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, _pattern.AttackStateName, out int layerIndex, out int stateHash, _pattern.AnimatorLayer))
                return;

            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, true);
            PatternAnimatorUtility.SetAttackBlendValue(animator, _pattern.BlendTreeValue, _pattern.AttackBlendParameter, _pattern.FallbackBlendParameter);
            animator.speed = 1f;
            animator.Play(stateHash, layerIndex, 0f);
            animator.Update(0f);
        }

        public void PlayIdleAnimation(Animator animator, int layerIndex)
        {
            if (animator == null || string.IsNullOrWhiteSpace(_pattern.IdleStateName))
                return;

            if (layerIndex < 0 || layerIndex >= animator.layerCount)
                return;

            if (!PatternAnimatorUtility.TryGetAnimatorStateOnLayer(animator, layerIndex, _pattern.IdleStateName, out int stateHash))
                return;

            animator.speed = 1f;
            animator.Play(stateHash, layerIndex, 0f);
            animator.Update(0f);
        }

        public float GetFullAttackDuration()
        {
            return _pattern.FullAttackClip != null
                ? Mathf.Max(MinAnimationDuration, _pattern.FullAttackClip.length)
                : FallbackFullAttackDuration;
        }

        public float GetFullAttackTriggerNormalizedTime(float animationDuration)
        {
            if (_pattern.FullAttackClip == null)
                return FallbackFullAttackTriggerNormalizedTime;

            foreach (AnimationEvent animationEvent in _pattern.FullAttackClip.events)
            {
                if (animationEvent.functionName != AnimationTriggerFunctionName)
                    continue;

                return Mathf.Clamp01(animationEvent.time / Mathf.Max(MinAnimationDuration, animationDuration));
            }

            return FallbackFullAttackTriggerNormalizedTime;
        }

        public void ResetAttackAnimation(Enemy owner)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (animator == null)
                return;

            animator.speed = 1f;
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, false);
        }
    }
}
