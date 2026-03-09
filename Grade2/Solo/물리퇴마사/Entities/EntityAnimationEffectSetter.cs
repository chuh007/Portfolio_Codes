using System;
using UnityEngine;

namespace _01Scripts.Entities
{
    public class EntityAnimationEffectSetter : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private GameObject[] effectObjs;
        
        private Entity _entity;
        private EntityAnimatorTrigger _entityAnimatorTrigger;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _entityAnimatorTrigger = _entity.GetCompo<EntityAnimatorTrigger>();
            _entityAnimatorTrigger.OnAttackEffectTrigger += HandleEffect;
            _entityAnimatorTrigger.OnAnimationEndTrigger += HandleEffectEnd;
            foreach (var obj in effectObjs)
            {
                obj.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _entityAnimatorTrigger.OnAttackEffectTrigger -= HandleEffect;
            _entityAnimatorTrigger.OnAnimationEndTrigger -= HandleEffectEnd;
        }

        private void HandleEffect()
        {
            foreach (var obj in effectObjs)
            {
                obj.SetActive(true);
            }
        }
        
        private void HandleEffectEnd()
        {
            foreach (var obj in effectObjs)
            {
                obj.SetActive(false);
            }
        }
    }
}