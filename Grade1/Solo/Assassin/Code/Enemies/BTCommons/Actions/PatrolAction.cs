using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Patrol", story: "[Self] Patrol with [mover] in [sec]", category: "Action", id: "51f7df61a72ba4478d35b04da7971ded")]
    public partial class PatrolAction : Action
    {
        [SerializeReference] public BlackboardVariable<BTEnemy> Self;
        [SerializeReference] public BlackboardVariable<EntityMover> Mover;
        [SerializeReference] public BlackboardVariable<float> Sec;

        private float _startTime;
        
        protected override Status OnStart()
        {
            Mover.Value.SetMovementX(Self.Value.FacingDirection);
            _startTime = Time.time;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            bool isOverTime = Sec.Value + _startTime < Time.time;
            bool isGround = Mover.Value.IsGroundDetected();
            
            if(isOverTime || isGround == false)
                return Status.Success;
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}

