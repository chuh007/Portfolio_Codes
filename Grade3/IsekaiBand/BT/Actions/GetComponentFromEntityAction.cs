using _Work.CHUH.Code.Enemies.Boss;
using System;
using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetComponentFromEntity", story: "get components in [Boss]", category: "Action", id: "68a6279e0b5753e8c76b1569b3d28965")]
public partial class GetComponentFromEntityAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    protected override Status OnStart()
    {
        Boss enemy = Boss.Value;
        //제네릭을 쓰면 이렇게 변경이 가능하다.
        SetVariableToBT(enemy, "Renderer", enemy.GetCompo<EntityRenderer>());
        SetVariableToBT(enemy, "Animator", enemy.GetCompo<EntityRenderer>().GetComponent<Animator>());
        SetVariableToBT(enemy, "Mover", enemy.GetCompo<EntityMover>());
        SetVariableToBT(enemy, "AnimatorTrigger", enemy.GetCompo<EntityAnimatorTrigger>());
        SetVariableToBT(enemy, "Target", enemy.target);
        // 이케 하니깐 두 적이 같은 BT를 쓸 수 이써요!!!!!!!!!!!!!!
            
        return Status.Success;
    }

    private void SetVariableToBT<T>(Boss enemy, string variableName, T component)
    {
        Debug.Assert(component != null, $"Check {variableName} component exist on {enemy.gameObject.name}");
        BlackboardVariable<T> variable = enemy.GetBlackboardVariable<T>(variableName);
        Debug.Assert(variable != null, $"Check {variableName}");

        variable.Value = component;
    }
}

