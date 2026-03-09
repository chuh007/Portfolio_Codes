using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;


namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move", story: "[self] move with [mover]", category: "Action", id: "fe564fb6ec4ebef9c55cabe657359125")]
    public partial class MoveAction : Action
    {
        [SerializeReference] public BlackboardVariable<BTEnemy> Self;
        [SerializeReference] public BlackboardVariable<EntityMover> Mover;

        protected override Status OnStart()
        {
            Mover.Value.SetMovementX(1);
            return Status.Success;
        }
    }
}

