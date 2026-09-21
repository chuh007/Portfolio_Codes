using _Work.CHUH.Code.BT;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/StateChange")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "StateChange", message: "enemy state change to [EnemyState]", category: "Events", id: "4855525161c767c0fb8f2fdfffe2feb0")]
public sealed partial class StateChange : EventChannel<BTEnemyState> { }

