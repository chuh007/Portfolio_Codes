using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DoubleDashAnimation
    {
        private readonly DoubleDashPatternSO _pattern;

        public DoubleDashAnimation(DoubleDashPatternSO pattern) => _pattern = pattern;

        public void PlayAttackAnimation(Enemy owner)
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
