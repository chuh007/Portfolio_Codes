using Chuh007Lib.Entities.Entities;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseToTarget", story: "[Self] chase to [Target] with [Mover]", category: "Action", id: "51420f9128deefd641daa193767828d2")]
public partial class ChaseToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<EntityMover> Mover;
    
    private EntityRenderer _renderer;

    protected override Status OnStart()
    {
        _renderer = Self.Value.GetComponentInChildren<EntityRenderer>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 movement = GetMovementDirection();
        Mover.Value.SetMovement(movement);
        _renderer?.FlipController(Target.Value.position.x - Self.Value.transform.position.x);
        return Status.Running;
    }
        
    private Vector2 GetMovementDirection()
    {
        Vector3 targetPosition = Target.Value.position;
        Vector3 myPosition = Self.Value.transform.position;
            
        Vector3 offset = targetPosition - myPosition;

        return offset.normalized;
    }
}

