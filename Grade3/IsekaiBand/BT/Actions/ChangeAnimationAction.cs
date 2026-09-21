using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeAnimation", story: "[Animator] change [Current] to [Next]", category: "Action", id: "e3c34ccea90949cb717b51c394d2e758")]
public partial class ChangeAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<string> Current;
    [SerializeReference] public BlackboardVariable<string> Next;

    protected override Status OnStart()
    {
        if(Current != null)  Animator.Value.SetBool(Current.Value, false);
        Current.Value = Next.Value;
        Animator.Value.SetBool(Current.Value, true);
        return Status.Success;
    }
}

