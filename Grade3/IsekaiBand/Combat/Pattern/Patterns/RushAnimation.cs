using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class RushAnimation
    {
        private const float LastFrameNormalizedTime = 0.999f;
        private const float MinAnimationDuration = 0.01f;
        private readonly RushPatternSO _pattern;
        public RushAnimation(RushPatternSO pattern) => _pattern = pattern;

        public RushAnimationState BeginAttackAnimation(Enemy owner, float normalizedTime)
        {
            return BeginAttackAnimation(owner, normalizedTime, true);
        }

        public RushAnimationState BeginAttackAnimation(Enemy owner, float normalizedTime, bool freezeAndSample)
        {
            return BeginAttackAnimation(owner, _pattern.AttackStateName, normalizedTime, freezeAndSample);
        }

        public RushAnimationState BeginAttackAnimation(
            Enemy owner,
            string stateName,
            float normalizedTime,
            bool freezeAndSample)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, stateName, out int layerIndex, out int stateHash, _pattern.AnimatorLayer))
                return default;

            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, true);
            PatternAnimatorUtility.SetAttackBlendValue(animator, _pattern.BlendTreeValue, _pattern.AttackBlendParameter, _pattern.FallbackBlendParameter);

            var state = new RushAnimationState(animator, layerIndex, stateHash, animator.speed);
            if (freezeAndSample)
            {
                animator.speed = 0f;
                SampleAttackAnimation(state, normalizedTime);
            }
            else
            {
                animator.Play(stateHash, layerIndex, 0f);
                animator.Update(0f);
            }
            return state;
        }

        public RushAnimationState BeginWarningAnimation(Enemy owner, float durationSeconds)
        {
            RushAnimationState state = BeginAttackAnimation(owner, _pattern.WarningStateName, 0f, false);
            if (state.IsValid)
            {
                state.Animator.speed = GetNormalizedWarningSpeed(durationSeconds);
                return state;
            }

            state = BeginAttackAnimation(owner, _pattern.LegacyWarningStateName, 0f, false);
            if (state.IsValid)
            {
                state.Animator.speed = GetNormalizedWarningSpeed(durationSeconds);
                return state;
            }

            state = BeginAttackAnimation(owner, _pattern.AttackStateName, 0f, false);
            if (state.IsValid)
                state.Animator.speed = GetNormalizedWarningSpeed(durationSeconds);

            return state;
        }

        public float GetNormalizedWarningSpeed(float durationSeconds)
        {
            return Mathf.Max(MinAnimationDuration, _pattern.AuthoredWarningDuration) /
                   Mathf.Max(MinAnimationDuration, durationSeconds);
        }

        public void SampleAttackAnimation(RushAnimationState state, float normalizedTime)
        {
            if (!state.IsValid)
                return;

            state.Animator.Play(state.StateHash, state.LayerIndex, Mathf.Clamp(normalizedTime, 0f, LastFrameNormalizedTime));
            state.Animator.Update(0f);
        }

        public void EndAttackAnimation(RushAnimationState state, bool resetBool)
        {
            if (!state.IsValid)
                return;

            state.Animator.speed = state.PreviousSpeed;
            if (resetBool)
                PatternAnimatorUtility.SetAnimatorBoolIfExists(state.Animator, _pattern.AttackBoolParameter, false);
        }
    }
}
