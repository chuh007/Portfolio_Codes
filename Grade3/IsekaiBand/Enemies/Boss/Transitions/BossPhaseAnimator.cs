using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal static class BossPhaseAnimator
    {
        private const float LastFrameNormalizedTime = 0.999f;

        public static async UniTask<bool> PlayAsync(
            Animator animator,
            int layerIndex,
            int stateHash,
            float duration,
            bool reverse,
            CancellationToken cancellationToken)
        {
            float previousSpeed = animator.speed;
            animator.speed = 0f;

            if (duration <= 0f)
            {
                SampleAnimatorState(animator, layerIndex, stateHash, reverse ? 0f : LastFrameNormalizedTime);
                animator.speed = previousSpeed;
                return false;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                float normalizedTime = reverse
                    ? Mathf.Lerp(LastFrameNormalizedTime, 0f, progress)
                    : Mathf.Lerp(0f, LastFrameNormalizedTime, progress);

                SampleAnimatorState(animator, layerIndex, stateHash, normalizedTime);

                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
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

        private static void SampleAnimatorState(Animator animator, int layerIndex, int stateHash, float normalizedTime)
        {
            animator.Play(stateHash, layerIndex, Mathf.Clamp01(normalizedTime));
            animator.Update(0f);
        }

        public static void SetBoolIfExists(Animator animator, string parameterName, bool value)
        {
            if (animator == null || string.IsNullOrWhiteSpace(parameterName))
                return;

            int hash = Animator.StringToHash(parameterName);
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.nameHash != hash || parameter.type != AnimatorControllerParameterType.Bool)
                    continue;

                animator.SetBool(hash, value);
                return;
            }
        }

        public static bool TryGetState(Animator animator, string stateName, out int layerIndex, out int stateHash)
        {
            layerIndex = 0;
            stateHash = 0;
            if (animator == null || string.IsNullOrWhiteSpace(stateName))
                return false;

            for (int i = 0; i < animator.layerCount; i++)
            {
                int fullPathHash = Animator.StringToHash($"{animator.GetLayerName(i)}.{stateName}");
                if (animator.HasState(i, fullPathHash))
                {
                    layerIndex = i;
                    stateHash = fullPathHash;
                    return true;
                }

                int shortNameHash = Animator.StringToHash(stateName);
                if (animator.HasState(i, shortNameHash))
                {
                    layerIndex = i;
                    stateHash = shortNameHash;
                    return true;
                }
            }

            return false;
        }
    }
}
