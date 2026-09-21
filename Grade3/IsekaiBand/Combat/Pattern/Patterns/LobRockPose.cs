using System.Threading;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LobRockPose
    {
        private readonly MiddleBossLobRockPatternSO _pattern;
        private const float LastFrameNormalizedTime = 0.999f;

        private readonly struct AttackAnimationState
        {
            public readonly Animator Animator;
            public readonly int LayerIndex;
            public readonly int StateHash;
            public readonly float PreviousSpeed;

            public bool IsValid => Animator != null;

            public AttackAnimationState(Animator animator, int layerIndex, int stateHash, float previousSpeed)
            {
                Animator = animator;
                LayerIndex = layerIndex;
                StateHash = stateHash;
                PreviousSpeed = previousSpeed;
            }
        }

        public LobRockPose(MiddleBossLobRockPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayAttackPoseAsync(Enemy owner, CancellationToken ct)
        {
            AttackAnimationState state = BeginAttackPose(owner);
            if (!state.IsValid)
                return;

            try
            {
                await UniTask.WaitForSeconds(_pattern.AttackPoseDuration, cancellationToken: ct);
            }
            finally
            {
                EndAttackPose(state);
            }
        }

        private AttackAnimationState BeginAttackPose(Enemy owner)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, _pattern.AttackStateName, out int layerIndex, out int stateHash, _pattern.AnimatorLayer))
                return default;

            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, true);

            var state = new AttackAnimationState(animator, layerIndex, stateHash, animator.speed);
            animator.speed = 0f;
            SampleAttackPose(state, _pattern.AttackFrameNormalizedTime);
            return state;
        }

        private void SampleAttackPose(AttackAnimationState state, float normalizedTime)
        {
            if (!state.IsValid)
                return;

            state.Animator.Play(state.StateHash, state.LayerIndex, Mathf.Clamp(normalizedTime, 0f, LastFrameNormalizedTime));
            state.Animator.Update(0f);
        }

        private void EndAttackPose(AttackAnimationState state)
        {
            if (!state.IsValid)
                return;

            state.Animator.speed = state.PreviousSpeed;
            PatternAnimatorUtility.SetAnimatorBoolIfExists(state.Animator, _pattern.AttackBoolParameter, false);
        }
    }
}
