using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Flip", story: "Flip [renderer]", category: "Action", id: "26794c393fbc2ffc9ffeb4bcb3e05558")]
    public partial class FlipAction : Action
    {
        [SerializeReference] public BlackboardVariable<EntityRenderer> Renderer;

        protected override Status OnStart()
        {
            Renderer.Value.Flip();
            return Status.Success;
        }
    }
}

