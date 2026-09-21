using Chuh007Lib.Entities.Entities;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopMove", story: "Stop [Move] Immediately", category: "Action", id: "e87e9e5f83a22531c85c1f5f38deeb2c")]
public partial class StopMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<EntityMover> Move;

    protected override Status OnStart()
    {
        Move.Value.StopImmediately();
        return Status.Success;
    }
    
}

