using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal readonly struct RushAnimationState
    {
            public readonly Animator Animator;
            public readonly int LayerIndex;
            public readonly int StateHash;
            public readonly float PreviousSpeed;

            public bool IsValid => Animator != null;

            public RushAnimationState(Animator animator, int layerIndex, int stateHash, float previousSpeed)
            {
                Animator = animator;
                LayerIndex = layerIndex;
                StateHash = stateHash;
                PreviousSpeed = previousSpeed;
            }

    }
}
