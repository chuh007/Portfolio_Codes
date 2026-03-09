using System;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "WaitForAnimationTrigger", story: "Wait for [trigger] end", category: "Action", id: "bcd8e789f25f4aa5b56054828e064ea6")]
    public partial class WaitForAnimationTriggerAction : Action
    {
        [SerializeReference] public BlackboardVariable<EntityAnimationTrigger> Trigger;

        private bool _animationEndTrigger;
    
        protected override Status OnStart()
        {
            _animationEndTrigger = false;
            Trigger.Value.OnAnimationEnd += HandleAnimationEnd;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _animationEndTrigger ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            Trigger.Value.OnAnimationEnd -= HandleAnimationEnd;
        }

        private void HandleAnimationEnd()
        {
            _animationEndTrigger = true;
        }
    }
}

