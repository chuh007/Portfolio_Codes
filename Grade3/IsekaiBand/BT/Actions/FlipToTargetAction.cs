using Chuh007Lib.Entities.Entities;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlipToTarget", story: "[Self] Facing [Target] to [Renderer]", category: "Action", id: "6bb1fe02a5e8b4aec66beee23da1d018")]
public partial class FlipToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Entity> Target;
    [SerializeReference] public BlackboardVariable<EntityRenderer> Renderer;
    protected override Status OnStart()
    {
        Renderer.Value.FlipController(Target.Value.transform.position.x - Self.Value.transform.position.x);
        return Status.Success;
    }
}

