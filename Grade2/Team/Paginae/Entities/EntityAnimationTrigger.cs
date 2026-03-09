using System;
using Blade.Entities;
using UnityEngine;

namespace Work.CUH.Code.Entities
{
    public class EntityAnimationTrigger : MonoBehaviour, IEntityComponent
    {
        public event Action OnAnimationEnd;
        
        public void Initialize(Entity entity)
        {
            
        }
        
        private void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }
    }
}