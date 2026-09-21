using System;
using System.Threading;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern
{
    [Serializable]
    public class PatternVanishPresentation
    {
        private const float LastFrameNormalizedTime = 0.999f;
        private const string AttackBoolParameter = "ATTACK";

        [SerializeField] private string deadStateName = "Dead";
        [SerializeField] private string deadBoolParameter = "DEAD";
        [SerializeField] private string returnStateName = "Idle";
        [SerializeField] private string returnBoolParameter = "IDLE";
        [SerializeField, Min(0f)] private float animationDuration = 0.7f;

        private PatternVanishAnimation _animation;
        private PatternVanishAnimation Animation => _animation ??= new PatternVanishAnimation(this);
        internal float AnimationDuration => animationDuration;

        public async UniTask<bool> VanishAsync(Enemy owner, CancellationToken ct)
        {
            EntityRenderer render = owner.GetCompo<EntityRenderer>();
            Animator animator = render != null ? render.Anim : owner.GetComponentInChildren<Animator>();
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, deadStateName, out int layerIndex, out int stateHash))
            {
                render?.SetAlpha(0f);
                return false;
            }

            render?.SetAlpha(1f);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, AttackBoolParameter, false);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, returnBoolParameter, false);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, deadBoolParameter, true);

            bool canceled = await Animation.PlayAnimatorStateAsync(animator, layerIndex, stateHash, false, ct);
            if (canceled)
                return true;

            PatternVanishAnimation.SampleAnimatorState(animator, layerIndex, stateHash, LastFrameNormalizedTime);
            animator.speed = 0f;
            return true;
        }

        public async UniTask ReturnAsync(Enemy owner, bool usedDeadAnimation, CancellationToken ct)
        {
            EntityRenderer render = owner.GetCompo<EntityRenderer>();
            Animator animator = render != null ? render.Anim : owner.GetComponentInChildren<Animator>();
            if (!usedDeadAnimation || !PatternAnimatorUtility.TryGetAnimatorState(animator, deadStateName, out int layerIndex, out int stateHash))
            {
                render?.SetAlpha(1f);
                return;
            }

            render?.SetAlpha(1f);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, AttackBoolParameter, false);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, returnBoolParameter, false);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, deadBoolParameter, true);

            bool canceled = await Animation.PlayAnimatorStateAsync(animator, layerIndex, stateHash, true, ct);
            if (canceled)
                return;

            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, deadBoolParameter, false);
            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, returnBoolParameter, true);
            PatternVanishAnimation.TryPlayAnimatorState(animator, returnStateName);
        }
    }
}
