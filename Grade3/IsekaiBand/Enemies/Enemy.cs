using System;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Level;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Card.Build;
using _Work.CHUH.Code.Combat;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.EntityPlus;
using _Work.CHUH.Code.EntityPlus.Effect;
using _Work.CHUH.Code.Item;
using _Work.CHUH.Code.Resource;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.Entities.Entities.FSM;
using Chuh007Lib.ObjectPool.RunTime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Enemies
{
    /// <summary>
    /// 모든 적의 조상 되는 클래스. 데미지를 받고 도트뎀 받고 느려지고 넉벡되고...
    /// </summary>
    public abstract class Enemy : Entity, IKnockbackable, ISlowable, ISlowStatus, IPoolable
    {
        private const float CoinRewardChance = 0.05f;
        private const int CoinRewardAmount = 1;
        private const string KnockbackReceivedMultiplierStatName = "knockbackReceivedMultiplier";

        [SerializeField] private ExpOrbDataSO expData;
        [SerializeField] private PoolItemSO expItem;
        [SerializeField] private int rewardLevel = 0;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private float knockbackDuration = 0.12f;
        
        [HideInInspector] public Entity target;
        
        protected Rigidbody2D _rbCompo;
        
        private IMover _mover;
        private EntityMover _entityMover;
        private IRenderer _renderer;
        private int _knockbackVersion;
        private int _activeKnockbackVersion;
        private bool _isKnockbackActive;
        private bool _isKnockbackImmune;
        private float _slowRemaining;
        private float _slowMultiplier = 1f;
        private float _damageDelay;
        private int _poolLifecycleVersion;
        private bool _isAutomaticRepositionSuppressed;
        private bool _defaultPhysicsSimulation;

        protected virtual bool AlwaysKnockbackImmune => false;
        public virtual bool IsBoss => false;
        public int PoolLifecycleVersion => _poolLifecycleVersion;
        public bool IsAutomaticRepositionSuppressed => _isAutomaticRepositionSuppressed;
        public Rigidbody2D PhysicsBody => _rbCompo;
        
        public bool IsKnockbackImmune
        {
            get => AlwaysKnockbackImmune || _isKnockbackImmune;
            set
            {
                if (AlwaysKnockbackImmune && !value) return;
                if (_isKnockbackImmune == value) return;

                _isKnockbackImmune = value;
                if (_isKnockbackImmune)
                {
                    _knockbackVersion++;
                    CancelActiveKnockback();
                }
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            _rbCompo = GetComponent<Rigidbody2D>();
            _defaultPhysicsSimulation = _rbCompo.simulated;
            _mover = GetComponentInChildren<IMover>();
            _entityMover = GetComponentInChildren<EntityMover>();
            _renderer = GetComponentInChildren<IRenderer>();
        }
        
        protected virtual void OnCollisionStay2D(Collision2D other)
        {
            if (_damageDelay < 0.1f) return;
            if (target.transform == other.transform) // 단순한 방식. 나중에 어그로 더미같은거 만들거면 방법을 바꿔야함
            {
                _damageDelay = 0;
                target.TakeDamage(new DamageData(1), Vector2.zero, this);
            }
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            _damageDelay += deltaTime;
            UpdateControlStatuses(deltaTime);
        }

        public void SetTarget(Entity target)
        {
            this.target = target;
            if (target != null)
                _renderer?.FlipController(target.transform.position.x - transform.position.x);
        }

        public void SetRewardCount(int count)
        {
            rewardLevel = Mathf.Max(0, count);
        }

        public void SetAutomaticRepositionSuppressed(bool suppressed)
        {
            _isAutomaticRepositionSuppressed = suppressed;
        }
        
        public async void ApplyKnockback(Vector2 direction, float force)
        {
            if (IsDead || IsKnockbackImmune) return;
            if (force <= 0f || direction.sqrMagnitude <= 0.0001f) return;
            if (_isKnockbackActive) return;

            int knockbackVersion = ++_knockbackVersion;
            int lifecycleVersion = _poolLifecycleVersion;
            float duration = Mathf.Max(0.01f, knockbackDuration);
            float multiplier = GetCompo<EntityStat>()?.GetValueByName(KnockbackReceivedMultiplierStatName) ?? 1f;
            Vector2 startVelocity = direction.normalized * (force * multiplier / Mathf.Max(_rbCompo.mass, 0.01f));
            var destroyToken = this.GetCancellationTokenOnDestroy();

            _isKnockbackActive = true;
            _activeKnockbackVersion = knockbackVersion;
            SetKnockbackMovementBlocked(true);

            try
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (knockbackVersion != _knockbackVersion) return;

                    float t = Mathf.Clamp01(elapsed / duration);
                    float speedMultiplier = 1f - Mathf.SmoothStep(0f, 1f, t);
                    _rbCompo.linearVelocity = startVelocity * speedMultiplier;

                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, destroyToken);
                    elapsed += Time.fixedDeltaTime;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (this != null
                    && lifecycleVersion == _poolLifecycleVersion
                    && _activeKnockbackVersion == knockbackVersion)
                    CancelActiveKnockback();
            }
        }

        public bool IsSlowed => _slowRemaining > 0f || (_entityMover != null && _entityMover.HasSlowEffect);

        public void ApplySlow(float multiplier, float duration)
        {
            if (duration <= 0f) return;

            multiplier = Mathf.Clamp01(multiplier);
            _slowMultiplier = _slowRemaining > 0f
                ? Mathf.Min(_slowMultiplier, multiplier)
                : multiplier;
            _slowRemaining = Mathf.Max(_slowRemaining, duration);
            _entityMover?.SetMoveSpeedMultiplier(_slowMultiplier);
        }

        public override void TakeDamage(DamageData damage, Vector2 direction = default, Entity dealer = null)
        {
            PlayerCommonBuildCompo sourceBuilds = null;
            if (dealer != null)
                dealer.TryGetComponent(out sourceBuilds);
            if (!damage.IsFixedDamage)
            {
                damage.Damage *= CommonBuildDebuffApplier.GetIncomingDamageMultiplier(
                    GetCompo<EntityEffectController>(), sourceBuilds);
            }

            base.TakeDamage(damage, direction, dealer);
        }
        
        protected override void HandleDead()
        {
            if(IsDead) return;
            CompleteDeath();
        }

        protected void CompleteDeath()
        {
            if (expItem != null)
            {
                if (expData != null)
                    ExperienceManager.Instance?.SpawnOrMergeOrb(poolManager, expItem, expData, transform.position);
                else
                {
                    UpgradeChest item = poolManager.Pop(expItem) as UpgradeChest;
                    if (item != null)
                    {
                        item.gameObject.transform.position = transform.position;
                        item.SetRewardLevel(rewardLevel);
                    }
                }
            }

            TryGiveCoinReward();
            EnterDeadState(true);
        }

        public void PlayDespawnDeathAnimation()
        {
            if (IsDead) return;

            EnterDeadState(false);
        }

        private void EnterDeadState(bool notifyDeath)
        {
            IsDead = true;
            ChangeState(StateName.Dead);
            gameObject.layer = DeadBodyLayer;
            StopDeathPhysics();
            if (notifyDeath)
                Bus<EnemyDeadEvent>.Raise(new EnemyDeadEvent(this));
        }

        protected void StopDeathPhysics()
        {
            _knockbackVersion++;
            CancelActiveKnockback();
            if (_mover != null)
            {
                _mover.StopImmediately();
                _mover.CanManualMove = false;
            }

            _rbCompo.linearVelocity = Vector2.zero;
            _rbCompo.angularVelocity = 0f;
            _rbCompo.simulated = false;
        }

        private static void TryGiveCoinReward()
        {
            float bonusChance = MetaUpgradeRuntime.GetValue(
                ExperienceManager.Instance != null ? ExperienceManager.Instance.PlayerTransform : null,
                MetaUpgradeType.MoreGold);
            float rewardChance = Mathf.Clamp01(CoinRewardChance + bonusChance);
            if (UnityEngine.Random.value >= rewardChance) return;

            SupplyManager.Instance?.AddCoin(CoinRewardAmount);
        }
        
        public virtual void ChangeState(StateName newState)
        {
        }

        public void ReturnToPool()
        {
            _poolLifecycleVersion++;
            _myPool.Push(this);
        }

        public bool IsCurrentPoolLifecycle(int lifecycleVersion)
        {
            return _poolLifecycleVersion == lifecycleVersion;
        }
        
        #region Pool

        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }

        private Pool _myPool;
        
        public virtual void ResetItem()
        {
            GetCompo<EntityEffectController>()?.ClearEffects();
            _poolLifecycleVersion++;
            _knockbackVersion++;
            _activeKnockbackVersion = 0;
            _isKnockbackActive = false;
            _isKnockbackImmune = false;
            _isAutomaticRepositionSuppressed = false;
            ResetControlStatuses();
            gameObject.layer = _defaultLayer;
            IsDead = false;
            _mover?.StopImmediately();
            _rbCompo.linearVelocity = Vector2.zero;
            _rbCompo.angularVelocity = 0f;
            _rbCompo.simulated = _defaultPhysicsSimulation;
            SetKnockbackMovementBlocked(false);
            if (_mover != null)
                _mover.CanManualMove = true;
            _renderer.SetAlpha(1);
            GetCompo<EntityStat>()?.ResetPooledStats();
            AfterInitialize(); // TODO 수정
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
        
        #endregion
        
        private void UpdateControlStatuses(float deltaTime)
        {
            if (_slowRemaining > 0f)
            {
                _slowRemaining -= deltaTime;
                if (_slowRemaining <= 0f)
                {
                    _slowRemaining = 0f;
                    _slowMultiplier = 1f;
                    _entityMover?.SetMoveSpeedMultiplier(1f);
                }
            }

        }

        private void ResetControlStatuses()
        {
            _slowRemaining = 0f;
            _slowMultiplier = 1f;
            _entityMover?.SetMoveSpeedMultiplier(1f);
        }

        private void CancelActiveKnockback()
        {
            if (!_isKnockbackActive)
                return;

            _isKnockbackActive = false;
            _activeKnockbackVersion = 0;
            if (_rbCompo != null)
                _rbCompo.linearVelocity = Vector2.zero;

            SetKnockbackMovementBlocked(false);
        }

        private void SetKnockbackMovementBlocked(bool blocked)
        {
            if (_entityMover != null)
            {
                _entityMover.SetMovementBlocked(this, blocked);
                return;
            }

            if (_mover != null)
                _mover.CanManualMove = !blocked && !IsDead;
        }

    }
}
