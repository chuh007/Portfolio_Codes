using System;
using System.Threading;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern
{
    internal class PatternVanishAnimation
    {
        private const float LastFrameNormalizedTime = 0.999f;
        private readonly PatternVanishPresentation _presentation;
        public PatternVanishAnimation(PatternVanishPresentation presentation) => _presentation = presentation;

        public async UniTask<bool> PlayAnimatorStateAsync(
            Animator animator,
            int layerIndex,
            int stateHash,
            bool reverse,
            CancellationToken ct)
        {
            float previousSpeed = animator.speed;
            animator.speed = 0f;

            if (_presentation.AnimationDuration <= 0f)
            {
                SampleAnimatorState(animator, layerIndex, stateHash, reverse ? 0f : LastFrameNormalizedTime);
                animator.speed = previousSpeed;
                return false;
            }

            float elapsed = 0f;
            while (elapsed < _presentation.AnimationDuration)
            {
                float progress = Mathf.Clamp01(elapsed / _presentation.AnimationDuration);
                float normalizedTime = reverse
                    ? Mathf.Lerp(LastFrameNormalizedTime, 0f, progress)
                    : Mathf.Lerp(0f, LastFrameNormalizedTime, progress);

                SampleAnimatorState(animator, layerIndex, stateHash, normalizedTime);

                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                if (canceled)
                {
                    animator.speed = previousSpeed;
                    return true;
                }

                elapsed += Time.deltaTime;
            }

            SampleAnimatorState(animator, layerIndex, stateHash, reverse ? 0f : LastFrameNormalizedTime);
            animator.speed = previousSpeed;
            return false;
        }

        public static void SampleAnimatorState(Animator animator, int layerIndex, int stateHash, float normalizedTime)
        {
            animator.Play(stateHash, layerIndex, Mathf.Clamp01(normalizedTime));
            animator.Update(0f);
        }

        public static bool TryPlayAnimatorState(Animator animator, string stateName)
        {
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, stateName, out int layerIndex, out int stateHash))
                return false;

            animator.speed = 1f;
            animator.Play(stateHash, layerIndex, 0f);
            return true;
        }
    }
}
