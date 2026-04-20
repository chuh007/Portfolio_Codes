using System;
using ChipmunkKingdoms.Scripts.Utility;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Work.Chipmunk._01.Scripts.Combat;
using Work.CHUH._01Scripts.Entities;

namespace Work.CHUH._01Scripts.Combat
{
    public abstract class EntityAttackCompo : MonoBehaviour, IContainerComponent, IAfterInitailze
    {
        [field: SerializeField]public LayerMask whatIsTarget { get; protected set; }
        
        [Header("Stat")]
        [SerializeField] private StatSO attackRangeStat;
        [SerializeField] private StatSO attackSpeedStat;
        [SerializeField] private StatSO damageStat;
        [SerializeField] private StatSO detectRangeStat;
        
        private readonly int attackSpeedHash = Animator.StringToHash("ATTACKSPEED");
        
        protected float _attackRange;
        protected float _attackSpeed;
        protected float _damage;
        protected float _detectRange;
        
        protected float _cooldownTimer;

        protected Entity _owner;
        protected EntityStat _statCompo;
        protected EntityAnimator _animator;
        protected EntityAnimatorTrigger _animatorTrigger;
        
        protected IDamageble _target;
        
        public bool IsCollTime => _cooldownTimer > 0f;
        
        public ComponentContainer ComponentContainer { get; set; }
        
        public void OnInitialize(ComponentContainer componentContainer)
        {

        }
        
        public void AfterInitailized()
        {
            _owner = ComponentContainer.Get<Entity>(true);
            _statCompo = ComponentContainer.Get<EntityStat>();
            _animator = ComponentContainer.Get<EntityAnimator>();
            _animatorTrigger = ComponentContainer.Get<EntityAnimatorTrigger>();
            _animatorTrigger.OnAttackTrigger += Attack;
            
            _damage = _statCompo.GetStat(damageStat).Value;
            _attackSpeed = _statCompo.GetStat(attackSpeedStat).Value;
            _attackRange = _statCompo.GetStat(attackRangeStat).Value;
            _detectRange = _statCompo.GetStat(detectRangeStat).Value;
            
            _statCompo.GetStat(damageStat).OnValueChanged += HandleDamageChange;
            _statCompo.GetStat(attackSpeedStat).OnValueChanged += HandleAttackSpeedChange;
            _statCompo.GetStat(attackRangeStat).OnValueChanged += HandleAttackRangeChange;
            _statCompo.GetStat(detectRangeStat).OnValueChanged += HandleDetectRangeChange;
            
            _animator.SetParam(attackSpeedHash, _attackSpeed);
        }

        private void HandleDamageChange(StatSO stat, float currentValue, float prevValue)
            => _damage = currentValue;

        private void HandleAttackSpeedChange(StatSO stat, float currentValue, float prevValue)
        {
            _attackSpeed = currentValue;
            _animator.SetParam(attackSpeedHash, _attackSpeed);    
        }

        private void HandleAttackRangeChange(StatSO stat, float currentValue, float prevValue)
            => _attackRange = currentValue;

        private void HandleDetectRangeChange(StatSO stat, float currentValue, float prevValue)
            => _detectRange = currentValue;
        
        protected void OnDestroy()
        {
            _animatorTrigger.OnAttackTrigger -= Attack;

            _statCompo.GetStat(damageStat).OnValueChanged -= HandleDamageChange;
            _statCompo.GetStat(attackSpeedStat).OnValueChanged -= HandleAttackSpeedChange;
            _statCompo.GetStat(attackRangeStat).OnValueChanged -= HandleAttackRangeChange;
            _statCompo.GetStat(detectRangeStat).OnValueChanged -= HandleDetectRangeChange;
        }
        
        private void Update()
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }
        
        public IDamageble FindCloseTarget()
        {
            Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, whatIsTarget);
            Transform closeTarget = null;
            foreach (var targetCollider in targetColliders)
            {
                if (closeTarget == null || Vector2.Distance(closeTarget.position, transform.position) >
                    Vector2.Distance(targetCollider.transform.position, transform.position))
                {
                    closeTarget = targetCollider.transform;
                }
            }
            if(closeTarget == null) return null;
            if (closeTarget.TryGetComponent(out ComponentContainer container))
            {
                var health = container.GetCompo<Health>();
                SetTarget(health);
                return health;
            }
            IDamageble damageable = closeTarget.GetComponent<IDamageble>();
            SetTarget(damageable);
            return damageable;
        }

        protected void SetTarget(IDamageble target)
            => _target = target;
        
        public bool IsInRange(Vector2 targetPos)
            => Mathf.Abs(targetPos.x - transform.position.x) <= _attackRange;
        
        public bool CanAttack(Vector2 targetPos)
        {
            return IsInRange(targetPos) && !IsCollTime;
        }
        
        protected virtual void Attack()
        {
            _cooldownTimer = 1 / _attackSpeed;
        }
    }
}