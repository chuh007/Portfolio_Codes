using System;
using UnityEngine;

namespace _01Scripts.Entities
{
    public class EntityAnimatorTrigger : MonoBehaviour, IEntityComponent
    {
        public Action OnAnimationEndTrigger;
        public Action OnAnimationEventTrigger;
        public Action OnAttackTrigger;
        public Action OnAttackEffectTrigger;
        
        private Entity _entity;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        private void AnimationEnd()
        {
            OnAnimationEndTrigger?.Invoke();
        }

        private void AttackTrigger()
        {
            OnAttackTrigger?.Invoke();
        }
        
        private void AttackEffectTrigger()
        {
            OnAttackEffectTrigger?.Invoke();
        }
    }
}