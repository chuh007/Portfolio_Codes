using Code.Enemies.BTCommons;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/StateChangeEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "StateChangeEvent", message: "enemy state change to [newState]", category: "Events", id: "33e453c328035eaf891875b430ed6d23")]
public partial class StateChangeEvent : EventChannelBase
{
    public delegate void StateChangeEventEventHandler(BTEnemyState newState);
    public event StateChangeEventEventHandler Event; 

    public void SendEventMessage(BTEnemyState newState)
    {
        Event?.Invoke(newState);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<BTEnemyState> newStateBlackboardVariable = messageData[0] as BlackboardVariable<BTEnemyState>;
        var newState = newStateBlackboardVariable != null ? newStateBlackboardVariable.Value : default(BTEnemyState);

        Event?.Invoke(newState);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        StateChangeEventEventHandler del = (newState) =>
        {
            BlackboardVariable<BTEnemyState> var0 = vars[0] as BlackboardVariable<BTEnemyState>;
            if(var0 != null)
                var0.Value = newState;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as StateChangeEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as StateChangeEventEventHandler;
    }
}

