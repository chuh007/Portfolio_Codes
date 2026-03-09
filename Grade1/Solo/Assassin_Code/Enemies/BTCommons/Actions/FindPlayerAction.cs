using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Code.Enemies;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindPlayer", story: "set [isPlayerFound] [SelfCompo]", category: "Action", id: "04c7163e7653988db26fba5f15064af4")]
public partial class FindPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> IsPlayerFound;
    [SerializeReference] public BlackboardVariable<BTEnemy> SelfCompo;
    protected override Status OnStart()
    {
        SelfCompo.Value.isPlayerFound = IsPlayerFound.Value;
        return Status.Success;
    }

}

