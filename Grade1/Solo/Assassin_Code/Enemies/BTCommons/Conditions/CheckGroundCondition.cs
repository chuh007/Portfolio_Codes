using System;
using Code.Entities;
using Unity.Behavior;
using UnityEngine;

namespace Code.Enemies.BTCommons.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckGround", story: "[mover] ground is [status]", category: "Conditions", id: "9653bda6826c784a78c9f3b4b99eafdd")]
    public partial class CheckGroundCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<EntityMover> Mover;
        [SerializeReference] public BlackboardVariable<bool> Status;

        public override bool IsTrue()
        {
            return Mover.Value.IsGroundDetected() == Status.Value;
        }

    }
}
