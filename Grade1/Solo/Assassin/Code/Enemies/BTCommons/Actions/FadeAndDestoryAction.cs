using Code.Entities;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FadeAndDestory", story: "Fade [renderer] in [sec] and destory [self]", category: "Action", id: "f359fda39848294ed877a4c342f248a6")]
    public partial class FadeAndDestoryAction : Action
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


