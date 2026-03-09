using System;
using Chipmunk.GameEvents;
using ChipmunkKingdoms.Scripts.Utility;
using Chuh007Lib.StatSystem;
using UnityEngine;
using Work.CHUH._01Scripts.Entities;
using Work.CHUH._01Scripts.Event;

namespace Work.CHUH._01Scripts.Combat
{
    public class EntityHealth : MonoBehaviour, IContainerComponent, IDamageble, IAfterInitailze
    {
        public Action<float, float> OnHPChange;
        
        [SerializeField] private StatSO hpStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        public Entity entity { get; private set; }
        private EntityStat _statCompo;

        public ComponentContainer ComponentContainer { get; set; }
        public void OnInitialize(ComponentContainer componentContainer)
        {
            
        }

        public void AfterInitailized()
        {
            entity = ComponentContainer.Get<Entity>(true);
            _statCompo = ComponentContainer.Get<EntityStat>();
            maxHealth = currentHealth = _statCompo.GetStat(hpStat).Value;
            OnHPChange?.Invoke(currentHealth, maxHealth);
            _statCompo.GetStat(hpStat).OnValueChanged += HandleMaxHPChange;
        }

        private void OnDestroy()
        {
        }

        public void ResetHealth()
        {
            currentHealth = _statCompo.GetStat(hpStat).Value;
            OnHPChange?.Invoke(currentHealth, maxHealth);
        }

        private void HandleMaxHPChange(StatSO stat, float currentvalue, float prevvalue)
        {
            float changed = currentvalue - prevvalue;
            maxHealth = currentvalue;
            if (changed > 0)
            {
                currentHealth = Mathf.Clamp(currentHealth + changed, 0, maxHealth);
            }
            else
            {
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            }
            OnHPChange?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if(entity.IsDead) return;
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            OnHPChange?.Invoke(currentHealth, maxHealth);
            EventBus<EntityHitEvent>.Raise(new EntityHitEvent(damage, transform.position));
            if (currentHealth <= 0)
            {
                entity.OnDeadEvent?.Invoke();
            }
            entity.OnHitEvent?.Invoke();
        }
    }
}