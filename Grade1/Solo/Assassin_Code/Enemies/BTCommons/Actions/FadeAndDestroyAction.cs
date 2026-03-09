using System;
using Code.Entities;
using DG.Tweening;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FadeAndDestroy", story: "fade [renderer] in [sec] and destroy [self]", category: "Action", id: "3a59e7f03e6c2bfc00a442c8be83883f")]
    public partial class FadeAndDestroyAction : Action
    {
        [SerializeReference] public BlackboardVariable<EntityRenderer> Renderer;
        [SerializeReference] public BlackboardVariable<float> Sec;
        [SerializeReference] public BlackboardVariable<GameObject> Self;

        protected override Status OnStart()
        {
            DOVirtual.DelayedCall(Sec.Value, () =>
            {
                Renderer.Value.SpriteRenderer.DOFade(0f, 1f).OnComplete(() =>
                {
                    GameObject.Destroy(Self.Value);
                });
            });
            return Status.Success;
        }
    }
}

