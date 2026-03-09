using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChange")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationChange", message: "change animation to [next]", category: "Events", id: "8ac573862f0f9a5749827a48234ec205")]
public partial class AnimationChange : EventChannelBase
{
    public delegate void AnimationChangeEventHandler(string next);
    public event AnimationChangeEventHandler Event; 

    public void SendEventMessage(string next)
    {
        Event?.Invoke(next);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<string> nextBlackboardVariable = messageData[0] as BlackboardVariable<string>;
        var next = nextBlackboardVariable != null ? nextBlackboardVariable.Value : default(string);

        Event?.Invoke(next);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AnimationChangeEventHandler del = (next) =>
        {
            BlackboardVariable<string> var0 = vars[0] as BlackboardVariable<string>;
            if(var0 != null)
                var0.Value = next;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AnimationChangeEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AnimationChangeEventHandler;
    }
}

