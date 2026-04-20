using System;
using Chipmunk.GameEvents;
using ChipmunkKingdoms.Scripts.Utility;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.RunTime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Work.CHUH._01Scripts.Combat;
using Work.CHUH._01Scripts.Enemies.Wave.GameEvents;
using Work.CHUH._01Scripts.Entities;
using Work.CHUH._01Scripts.Event;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace Work.CHUH._01Scripts.Enemies
{
    public abstract class Enemy : Entity, IPoolable, IAfterInitailze, IStunnable
    {
        
        #region Pooling

        public UnityEvent ResetEvent;
        
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        
        private Pool _myPool;
        public void ResetItem()
        {
            IsDead = false;
            IsStun = false;
            OnDeadEvent.RemoveAllListeners();
            OnHitEvent.AddListener(OnHit);
            OnDeadEvent.AddListener(OnDead);
            _colliderCompo.enabled = true;
            ChangeAnimation(idleHash);
            ResetEvent?.Invoke();
            TargetFindLoop();
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        #endregion

        [field:SerializeField] public bool IsFlying { get; private set; }
        [SerializeField] private int dropCoin = 5;
        
        protected IDamageble _target;
        protected Vector3 _targetPos;
        protected Vector3 _targetDir;
        
        protected EntityMover _entityMover;
        protected EntityAttackCompo _entityAttackCompo;
        protected EntityAnimator _entityAnimator;
        protected EntityStat _entityStat;

        protected Rigidbody2D _rbCompo;
        protected Collider2D _colliderCompo;
        
        protected int _currentAnimationHash = Animator.StringToHash("IDLE");

        #region Hash

        private readonly int idleHash = Animator.StringToHash("IDLE");
        private readonly int moveHash = Animator.StringToHash("MOVE");
        private readonly int attackHash = Animator.StringToHash("ATTACK");
        private readonly int stunHash = Animator.StringToHash("STUN");
        private readonly int deadHash = Animator.StringToHash("DEAD");

        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            OnHitEvent.AddListener(OnHit);
            OnDeadEvent.AddListener(OnDead);
            EventBus<WaveEndEvent>.OnEvent += HandleWaveEnd;
        }
        
        private void OnDestroy()
        {
            OnHitEvent.RemoveListener(OnHit);
            OnDeadEvent.RemoveListener(OnDead);
            EventBus<WaveEndEvent>.OnEvent -= HandleWaveEnd;
        }

        protected override void Start()
        {
            base.Start();
            TargetFindLoop();
        }
        
        public void AfterInitailized()
        {
            _entityMover = ComponentContainer.Get<EntityMover>();
            _entityAttackCompo = ComponentContainer.Get<EntityAttackCompo>(true);
            _rbCompo = GetComponent<Rigidbody2D>();
            _colliderCompo = GetComponent<Collider2D>();
            _entityAnimator = ComponentContainer.Get<EntityAnimator>();
            _entityStat = ComponentContainer.Get<EntityStat>();
        }
        
        private async void TargetFindLoop()
        {
            while (!IsDead)
            {
                await Awaitable.WaitForSecondsAsync(0.5f);
                var target = _entityAttackCompo.FindCloseTarget();
                SetTarget(target);
            }
        }
        
        public void SetTarget(IDamageble target)
        {
            _target = target;
            if (target != null)
            {
                var targetCol = _target.gameObject.GetComponentInParent<Collider2D>();
                if(!targetCol) return;
                _targetPos = targetCol.ClosestPoint(transform.position);
                _targetDir = (_targetPos - transform.position).normalized;
            }
            else _targetDir = Vector3.left;
            
        }

        public void MultiplyStat(string statName, int waveCount)
        {
            float value = Mathf.Sqrt(waveCount) * 0.5f + waveCount * 0.05f;
            value = value > 1f ? value : 1f;
            const string wave = "Wave";
            _entityStat.AddStat(statName, wave, value);
        }
        
        protected virtual void Update()
        {
            if (IsDead) return;
            if (IsStun) return;
            if (_target != null && _entityAttackCompo.IsInRange(_targetPos))
            {
                _entityMover.Stop();
                if (_entityAttackCompo.CanAttack(_targetPos))
                {
                    ChangeAnimation(attackHash);
                }
            }
            else
            {
                if(_currentAnimationHash != moveHash) ChangeAnimation(moveHash);
                _entityMover.Move(_targetDir);
            }
        }
        
        private void ChangeAnimation(int newHash)
        {
             _entityAnimator.SetParam(_currentAnimationHash, false);
             _currentAnimationHash = newHash;
             _entityAnimator.SetParam(_currentAnimationHash, true);
        }
        
        private void OnHit()
        {
            if (IsDead) return;
        }

        private void OnDead()
        {
            if(IsDead) return;
            IsDead = true;
            _entityStat.ClearAllStatChange();
            _entityMover.Stop();
            _colliderCompo.enabled = false;
            EventBus<EnemyDeadEvent>.Raise(new EnemyDeadEvent(dropCoin));
            ChangeAnimation(deadHash);
            DOVirtual.DelayedCall(1f, () =>
            {
                _myPool.Push(this);
            });
        }
        
        private void HandleWaveEnd(WaveEndEvent evt)
        {
            OnDead();
        }
        
        public bool IsStun { get; private set; }
        public Tween StunTween { get; private set; }
        public float CurrentStunTime { get; private set; }
        
        public void Stun(float stunTime)
        {
            if(CurrentStunTime > stunTime) return;
            ChangeAnimation(stunHash);
            StunTween?.Kill();
            _entityMover.Stop();
            IsStun = true;
            CurrentStunTime = stunTime;
            StunTween = DOVirtual.DelayedCall(stunTime, () =>
            {
                CurrentStunTime = 0f;
                IsStun = false;
            });
        }
    }
}