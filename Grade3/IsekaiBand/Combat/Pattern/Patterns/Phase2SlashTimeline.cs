using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashTimeline
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;
        private const float LastFrameNormalizedTime = 0.999f;
        private const float MinAnimationDuration = 0.01f;
        public Phase2SlashTimeline(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayAnimatorStateWithTrigger(
            Animator animator,
            int layerIndex,
            int stateHash,
            float duration,
            float triggerNormalizedTime,
            bool reverse,
            System.Action onTrigger,
            CancellationToken ct)
        {
            float previousSpeed = animator.speed > MinAnimationDuration ? animator.speed : 1f;
            animator.speed = 0f;

            duration = Mathf.Max(MinAnimationDuration, duration);
            if (duration <= 0f)
            {
                SampleAnimatorState(animator, layerIndex, stateHash, reverse ? 0f : LastFrameNormalizedTime);
                animator.speed = Mathf.Max(MinAnimationDuration, previousSpeed);
                return;
            }

            float elapsed = 0f;
            float previousNormalizedTime = reverse ? LastFrameNormalizedTime : 0f;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                float normalizedTime = reverse
                    ? Mathf.Lerp(LastFrameNormalizedTime, 0f, progress)
                    : Mathf.Lerp(0f, LastFrameNormalizedTime, progress);

                SampleAnimatorState(animator, layerIndex, stateHash, normalizedTime);
                if (DidCrossTrigger(previousNormalizedTime, normalizedTime, triggerNormalizedTime, reverse))
                    onTrigger?.Invoke();

                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                if (canceled)
                {
                    animator.speed = previousSpeed;
                    return;
                }

                elapsed += Time.deltaTime;
                previousNormalizedTime = normalizedTime;
            }

            float finalNormalizedTime = reverse ? 0f : LastFrameNormalizedTime;
            SampleAnimatorState(animator, layerIndex, stateHash, finalNormalizedTime);
            if (DidCrossTrigger(previousNormalizedTime, finalNormalizedTime, triggerNormalizedTime, reverse))
                onTrigger?.Invoke();

            animator.speed = Mathf.Max(MinAnimationDuration, previousSpeed);
        }

        public static void SampleAnimatorState(Animator animator, int layerIndex, int stateHash, float normalizedTime)
        {
            animator.Play(stateHash, layerIndex, Mathf.Clamp01(normalizedTime));
            animator.Update(0f);
        }

        public static float GetTimeUntilTrigger(
            float animationDuration,
            float triggerNormalizedTime,
            bool reverse)
        {
            float startNormalizedTime = reverse ? LastFrameNormalizedTime : 0f;
            float distance = reverse
                ? startNormalizedTime - triggerNormalizedTime
                : triggerNormalizedTime - startNormalizedTime;

            return Mathf.Max(0f, distance * animationDuration);
        }

        public static bool DidCrossTrigger(
            float previousNormalizedTime,
            float normalizedTime,
            float triggerNormalizedTime,
            bool reverse)
        {
            if (reverse)
                return previousNormalizedTime >= triggerNormalizedTime && normalizedTime <= triggerNormalizedTime;

            return previousNormalizedTime <= triggerNormalizedTime && normalizedTime >= triggerNormalizedTime;
        }
    }
}
