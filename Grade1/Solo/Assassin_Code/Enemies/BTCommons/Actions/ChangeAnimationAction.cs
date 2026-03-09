using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "ChangeAnimation", story: "[animator] change [current] to [next]", category: "Action", id: "0c506a46f6662f584f18451ed902f2c8")]
    public partial class ChangeAnimationAction : Action
    {
        [SerializeReference] public BlackboardVariable<Animator> Animator;
        [SerializeReference] public BlackboardVariable<string> Current;
        [SerializeReference] public BlackboardVariable<string> Next;

        protected override Status OnStart()
        {
            Animator.Value.SetBool(Current.Value, false);
            Current.Value = Next.Value;
            Animator.Value.SetBool(Current.Value, true);
            return Status.Success;
        }
    }
}

