using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChangeEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationChangeEvent", message: "change animation to [StateName]", category: "Events", id: "c16ed7866b573f356bbb26ac26aed673")]
public sealed partial class AnimationChangeEvent : EventChannel<string> { }

