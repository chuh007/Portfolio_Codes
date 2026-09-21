using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern
{
    internal static class PatternAnimatorUtility
    {
        public static Animator GetAnimator(Enemy owner)
        {
            if (owner == null)
                return null;

            EntityRenderer renderer = owner.GetCompo<EntityRenderer>();
            return renderer != null ? renderer.Anim : owner.GetComponentInChildren<Animator>();
        }

        public static bool TryGetAnimatorState(Animator animator, string stateName, out int layerIndex, out int stateHash, int animatorLayer = -1)
        {
            layerIndex = 0;
            stateHash = 0;
            if (animator == null || string.IsNullOrWhiteSpace(stateName))
                return false;

            if (animatorLayer >= 0 && animatorLayer < animator.layerCount)
            {
                if (TryGetAnimatorStateOnLayer(animator, animatorLayer, stateName, out stateHash))
                {
                    layerIndex = animatorLayer;
                    return true;
                }
            }

            for (int i = 0; i < animator.layerCount; i++)
            {
                if (!TryGetAnimatorStateOnLayer(animator, i, stateName, out stateHash))
                    continue;

                layerIndex = i;
                return true;
            }

            return false;
        }

        public static bool TryGetAnimatorStateOnLayer(
            Animator animator,
            int layerIndex,
            string stateName,
            out int stateHash)
        {
            stateHash = Animator.StringToHash($"{animator.GetLayerName(layerIndex)}.{stateName}");
            if (animator.HasState(layerIndex, stateHash))
                return true;

            stateHash = Animator.StringToHash(stateName);
            return animator.HasState(layerIndex, stateHash);
        }

        public static void SetAttackBlendValue(Animator animator, int blendValue, string attackBlendParameter, string fallbackBlendParameter)
        {
            if (animator == null)
                return;

            if (TrySetFloat(animator, attackBlendParameter, blendValue))
                return;

            TrySetFloat(animator, fallbackBlendParameter, blendValue);
        }

        public static bool TrySetFloat(Animator animator, string parameterName, float value)
        {
            if (animator == null || string.IsNullOrWhiteSpace(parameterName))
                return false;

            int hash = Animator.StringToHash(parameterName);
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.nameHash != hash || parameter.type != AnimatorControllerParameterType.Float)
                    continue;

                animator.SetFloat(hash, value);
                return true;
            }

            return false;
        }

        public static void SetAnimatorBoolIfExists(Animator animator, string parameterName, bool value)
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
    }
}
