using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChange")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationChange", message: "change animation to [next]", category: "Events", id: "7ced831f6527aca9579e2622ffec45b7")]
public partial class AnimationChangeEvent : EventChannelBase
{
    public delegate void AnimationChangeEventEventHandler(string next);
    public event AnimationChangeEventEventHandler Event; 

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
        AnimationChangeEventEventHandler del = (next) =>
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
        Event += del as AnimationChangeEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AnimationChangeEventEventHandler;
    }
}

