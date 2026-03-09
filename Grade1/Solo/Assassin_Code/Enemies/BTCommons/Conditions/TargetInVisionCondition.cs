using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TargetInVision", story: "[self] check [target] in vision [viewAngle]", category: "Conditions", id: "b68bc760b133971aed5de207883ed7f1")]
public partial class TargetInVisionCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> ViewAngle;

    public override bool IsTrue()
    {
        Vector3 targetDir = (Target.Value.position - Self.Value.transform.position).normalized;
        float dot = Vector3.Dot(Self.Value.transform.right, targetDir);
        float value = Mathf.Cos((ViewAngle.Value / 2f) * Mathf.Deg2Rad);
        return dot >= value;
    }
}
