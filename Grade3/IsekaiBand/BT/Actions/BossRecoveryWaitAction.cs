using System;
using _Work.CHUH.Code.Enemies.Boss;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossRecoveryWait", story: "[Self] wait boss recovery", category: "Action", id: "5f698dbcd1f54b3eb9d58d40ec6a6352")]
public partial class BossRecoveryWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeField, Min(0f)] private float commonWaitDuration = 0.25f;

    private float _endTime;

    protected override Status OnStart()
    {
        float duration = commonWaitDuration;
        Boss boss = Self?.Value != null ? Self.Value.GetComponent<Boss>() : null;
        if (boss != null)
            duration = boss.ConsumePatternRecoveryDelay(commonWaitDuration);

        _endTime = Time.time + Mathf.Max(0f, duration);
        return Time.time >= _endTime ? Status.Success : Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Time.time >= _endTime ? Status.Success : Status.Running;
    }
}
