using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "ChaseToTarget", story: "[self] chase to [target] with [mover] [speedMult]", category: "Action", id: "33a0d6f88580af3aabbd6998842d56f6")]
    public partial class ChaseToTargetAction : Action
    {
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<EntityMover> Mover;
    [SerializeReference] public BlackboardVariable<float> SpeedMult;
        protected override Status OnStart()
        {
            Mover.Value.SetMoveSpeedMultiplier(SpeedMult.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            float xMove = GetMovementXDirection();
            Mover.Value.SetMovementX(xMove);
            return Status.Running;
        }

        private float GetMovementXDirection()
        {
            Vector3 targetPosition = Target.Value.position;
            Vector3 myPosition = Self.Value.transform.position;
            
            Vector3 offset = targetPosition - myPosition;
            
            return offset.x;
        }
    }
}

