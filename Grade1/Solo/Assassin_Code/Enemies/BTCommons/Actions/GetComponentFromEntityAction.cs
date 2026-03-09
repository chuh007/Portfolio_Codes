using System;
using Code.Enemies.Skeleton;
using Code.Entities;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Enemies.BTCommons.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "GetComponentFromEntity", story: "Get components from [btEnemy]", category: "Action", id: "0b297f7eaecf12a6c7605f2ac811de7e")]
    public partial class GetComponentFromEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<BTEnemy> BtEnemy;

        protected override Status OnStart()
        {
            BTEnemy enemy = BtEnemy.Value;
            //제네릭을 쓰면 이렇게 변경이 가능하다.
            SetVariableToBT(enemy, "Renderer", enemy.GetCompo<EntityRenderer>());
            SetVariableToBT(enemy, "MainAnimator", enemy.GetCompo<EntityRenderer>().GetComponent<Animator>());
            SetVariableToBT(enemy, "Mover", enemy.GetCompo<EntityMover>());
            SetVariableToBT(enemy, "AnimationTrigger", enemy.GetCompo<EntityAnimationTrigger>());
            SetVariableToBT(enemy, "SelfCompo", enemy.GetComponent<BTEnemy>());
            
            return Status.Success;
        }

        private void SetVariableToBT<T>(BTEnemy enemy, string variableName, T component)
        {
            Debug.Assert(component != null, $"Check {variableName} component exist on {enemy.gameObject.name}");
            BlackboardVariable<T> variable = enemy.GetBlackboardVariable<T>(variableName);
            variable.Value = component;
        }

    }
}

/*
EntityRenderer rendererCompo = enemy.GetCompo<EntityRenderer>();
Debug.Assert(rendererCompo != null, $"Check render component exist on {enemy.gameObject.name}");
BlackboardVariable<EntityRenderer> renderer = enemy.GetBlackboardVariable<EntityRenderer>("Renderer");
renderer.Value = rendererCompo;

BlackboardVariable<Animator> mainAnimator = enemy.GetBlackboardVariable<Animator>("MainAnimator");
mainAnimator.Value = rendererCompo.GetComponent<Animator>();

BlackboardVariable<EntityMover> mover = enemy.GetBlackboardVariable<EntityMover>("Mover");
EntityMover moverCompo = enemy.GetCompo<EntityMover>();
Debug.Assert(moverCompo != null, $"Check mover component exist on {enemy.gameObject.name}");
mover.Value = moverCompo;

BlackboardVariable<EntityAnimationTrigger> animationTrigger 
    = enemy.GetBlackboardVariable<EntityAnimationTrigger>("AnimationTrigger");
EntityAnimationTrigger triggerCompo = enemy.GetCompo<EntityAnimationTrigger>();
Debug.Assert(triggerCompo != null, $"Check animation trigger component exist on {enemy.gameObject.name}");
animationTrigger.Value = triggerCompo;
*/