using _Work.CHUH.Code.Enemies.Boss;
using Chuh007Lib.ObjectPool.RunTime;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PushSelf", story: "[Self] push [Pool]", category: "Action", id: "eaf9a6ccb0859a3710be4ef223667305")]
public partial class PushSelfAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Self;
    [SerializeReference] public BlackboardVariable<PoolManagerSO> Pool;

    protected override Status OnStart()
    {
        Pool.Value.Push(Self.Value);
        return Status.Success;
    }
}

