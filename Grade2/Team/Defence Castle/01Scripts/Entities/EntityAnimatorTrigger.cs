using System;
using ChipmunkKingdoms.Scripts.Utility;
using UnityEngine;

namespace Work.CHUH._01Scripts.Entities
{
    public partial class EntityAnimatorTrigger : MonoBehaviour, IContainerComponent
    {
        public Action OnAnimationEndTrigger;
        public Action OnAttackTrigger;
        
        public ComponentContainer ComponentContainer { get; set; }
        public void OnInitialize(ComponentContainer componentContainer)
        {
        }
        
        private void AnimationEnd()
        {
            OnAnimationEndTrigger?.Invoke();
        }

        private void AttackTrigger()
        {
            OnAttackTrigger?.Invoke();
        }
    }
}