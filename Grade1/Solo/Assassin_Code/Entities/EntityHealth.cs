using System;
using Code.Combats;
using Code.Core.EventSystems;
using Code.Core.StatSystem;
using UnityEngine;

namespace Code.Entities
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IAfterInit
    {
        [SerializeField] private StatSO hpStat;
        [SerializeField] private GameEventChannelSO uiChannel;
        public float maxHealth;
        private float _currentHealth;
        
        public event Action<Vector2> OnKnockback;
        
        private Entity _entity;
        private EntityStat _statCompo;
        private EntityFeedbackData _feedbackData;

        #region Initialize section

        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = _entity.GetCompo<EntityStat>();
            _feedbackData = _entity.GetCompo<EntityFeedbackData>();
        }
        
        public void AfterInit()
        {
            _statCompo.GetStat(hpStat).OnValueChange += HandleHPChange;
            _currentHealth = maxHealth = _statCompo.GetStat(hpStat).Value;
            _entity.OnDamage += ApplyDamage;
        }

        private void OnDestroy()
        {
            _statCompo.GetStat(hpStat).OnValueChange -= HandleHPChange;
            _entity.OnDamage -= ApplyDamage;
        }

        #endregion
        

        private void HandleHPChange(StatSO stat, float current, float previous)
        {
            maxHealth = current;
            _currentHealth = Mathf.Clamp(_currentHealth + current - previous, 1f, maxHealth);
            //체력변경으로 인해 사망하는 일은 없도록
            HPValueChangeEvent hpValueChangeEvent = UIEvents.HPValueChangeEvent;
            hpValueChangeEvent.value = _currentHealth;
            hpValueChangeEvent.maxValue = maxHealth;
            uiChannel.RaiseEvent(hpValueChangeEvent);
        }

        public void ApplyDamage(float damage, Vector2 direction, Vector2 knockBackPower, bool isPowerAttack, Entity dealer)
        {
            if (_entity.IsDead) return; //이미 죽은 녀석입니다.
            
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
            _feedbackData.LastAttackDirection = direction.normalized;
            _feedbackData.IsLastHitPowerAttack = isPowerAttack;
            _feedbackData.LastEntityWhoHit = dealer;
            HPValueChangeEvent hpValueChangeEvent = UIEvents.HPValueChangeEvent;
            hpValueChangeEvent.value = _currentHealth;
            hpValueChangeEvent.maxValue = maxHealth;
            uiChannel?.RaiseEvent(hpValueChangeEvent);

            AfterHitFeedbacks(knockBackPower);
        }


        private void AfterHitFeedbacks(Vector2 knockBackPower)
        {
            _entity.OnHit?.Invoke();
            OnKnockback?.Invoke(knockBackPower);

            if (_currentHealth <= 0)
            {
                _entity.OnDead?.Invoke();
            }
        }
    }
}