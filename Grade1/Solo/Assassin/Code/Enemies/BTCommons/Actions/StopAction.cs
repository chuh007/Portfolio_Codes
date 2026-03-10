using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop", story: "stop with [mover] on [yAxis]", category: "Action", id: "1783312693b5ad6c82e4a09b7a2c203b")]
    public partial class StopAction : Action
    {
        [SerializeReference] public BlackboardVariable<EntityMover> Mover;
        [SerializeReference] public BlackboardVariable<bool> YAxis;

        protected override Status OnStart()
        {
            Mover.Value.StopImmediately(YAxis.Value);
            return Status.Success;
        }
    }
}

