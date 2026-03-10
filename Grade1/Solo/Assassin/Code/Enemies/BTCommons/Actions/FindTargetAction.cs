using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FindTarget", story: "[self] set [target] from finder", category: "Action", id: "38bbe95f7980e52db191ddb238dd2e30")]
    public partial class FindTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<BTEnemy> Self;
        [SerializeReference] public BlackboardVariable<Transform> Target;

        protected override Status OnStart()
        {
            Target.Value = Self.Value.PlayerFinder.target.transform;
            Debug.Assert(Target.Value != null, $"Target is null : {Self.Value.gameObject.name}");
            
            return Status.Success;
        }
    }
}

