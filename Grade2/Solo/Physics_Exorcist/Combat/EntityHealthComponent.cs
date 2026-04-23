using _01Scripts.Entities;
using _01Scripts.Players;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01Scripts.Combat
{
    public class EntityHealthComponent : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        public UnityEvent<float> currentHpValueChangeEvent;
        
        [SerializeField] private StatSO hpStat;
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Transform damageCanvasTrm;
        public float maxHealth;
        private float _currentHealth;
        
        private Entity _entity;
        private EntityStat _statCompo;
        private EntityFeedbackData _feedbackData;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = _entity.GetCompo<EntityStat>();
            _feedbackData = _entity.GetCompo<EntityFeedbackData>();
        }
        
        public void AfterInitialize()
        {
            _statCompo.GetStat(hpStat).OnValueChanged += HandleHPChange;
            _currentHealth = maxHealth = _statCompo.GetStat(hpStat).Value;
            currentHpValueChangeEvent?.Invoke(_currentHealth);
            _entity.OnDamage += ApplyDamage;
        }

        private void OnDestroy()
        {
            _statCompo.GetStat(hpStat).OnValueChanged -= HandleHPChange;
            _entity.OnDamage -= ApplyDamage;
        }
        
        private void HandleHPChange(StatSO stat, float current, float previous)
        {
            maxHealth = current;
            _currentHealth = Mathf.Clamp(_currentHealth + current - previous, 1f, maxHealth);
            currentHpValueChangeEvent?.Invoke(_currentHealth);
        }
        
        private void ApplyDamage(DamageData damageData, Entity dealer)
        {
            if (_entity.IsDead) return;
            DamageText text = Instantiate(damageTextPrefab, damageCanvasTrm).GetComponent<DamageText>();
            text.SetDamageAndPos(damageData.damage, _entity.transform.position);
            _currentHealth = Mathf.Clamp(_currentHealth - damageData.damage, 0, maxHealth);
            currentHpValueChangeEvent?.Invoke(_currentHealth);
            _feedbackData.LastEntityWhoHit = dealer;
            AfterHitFeedbacks();
        }

        public void ApplyHeal(float heal)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + maxHealth * (heal / 100f), 0, maxHealth);
            currentHpValueChangeEvent?.Invoke(_currentHealth);
        }
        
        private void AfterHitFeedbacks()
        {
            _entity.OnHit?.Invoke();
            if (_currentHealth <= 0)
            {
                _entity.OnDead?.Invoke(_entity);
            }
        }

        public void RestoreHealth(float restoreHealth)
        {
            _currentHealth = restoreHealth;
            currentHpValueChangeEvent?.Invoke(_currentHealth);
        }

    }
}