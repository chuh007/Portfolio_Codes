using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FacingToTarget", story: "[self] facing to [target] with [renderer]", category: "Action", id: "6ed657baeae9ebedde1cd951b2003ee2")]
    public partial class FacingToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<Transform> Self;
        [SerializeReference] public BlackboardVariable<Transform> Target;
        [SerializeReference] public BlackboardVariable<EntityRenderer> Renderer;

        protected override Status OnStart()
        {
            Vector2 direction = Target.Value.position - Self.Value.position;
        
            Renderer.Value.FlipController(Mathf.Sign(direction.x));
        
            return Status.Success;
        }

    }
}

