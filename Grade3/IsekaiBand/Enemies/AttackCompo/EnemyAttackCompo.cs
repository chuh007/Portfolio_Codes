using System;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Pattern;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Enemies.AttackCompo
{
    /// <summary>
    /// 적의 공격 기반
    /// </summary>
    public abstract class EnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitalize
    {
        [SerializeField] protected LayerMask whatIsTarget;
        [Header("Stats")]
        [SerializeField] private StatSO attackDamageStat;
        [SerializeField] private StatSO attackRangeStat;
        [SerializeField] private StatSO attackCooldownStat;
        [Header("Pattern")]
        [SerializeField] private BasePatternSO singlePattern;

        protected EntityStat _stat;
        protected Enemy _enemy;
        protected IDamageable _target;
        protected float _lastAtkTime;

        protected int _damage = 5;
        protected float _attackRange = 10f;
        protected float _attackCooldown = 2f;

        private CancellationTokenSource _singlePatternCts;

        public bool HasSinglePattern => singlePattern != null;
        public bool IsSinglePatternRunning { get; private set; }
        
        public void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
            _stat = entity.GetCompo<EntityStat>();
        }
        
        public virtual void AfterInitialize()
        {
            _target = _enemy.target;
            _damage = (int)_stat.GetStat(attackDamageStat).Value;
            _attackRange = _stat.GetStat(attackRangeStat).Value;
            _attackCooldown = _stat.GetStat(attackCooldownStat).Value;
        }

        private void OnDestroy()
        {
        }

        protected virtual void OnDisable()
        {
            CancelSinglePattern();
        }
        
        // 공격이 가능한지
        public virtual bool IsTargetInAttackRange()
        {
            _target ??= _enemy.target;
            return _target != null &&
                   Vector2.Distance(_target.transform.position, transform.position) <= _attackRange;
        }

        public virtual bool CanAttack()
        {
            return !IsSinglePatternRunning &&
                   _lastAtkTime + _attackCooldown < Time.time &&
                   IsTargetInAttackRange();
        }
        
        public virtual void Attack(Action onAttackEnd = null)
        {
            _lastAtkTime = Time.time;
        }

        protected bool TryUseSinglePattern(Action onAttackEnd = null)
        {
            if (singlePattern == null) return false;

            _lastAtkTime = Time.time;
            CancelSinglePattern();

            _singlePatternCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            IsSinglePatternRunning = true;
            UseSinglePatternAsync(singlePattern, onAttackEnd, _singlePatternCts).Forget();
            return true;
        }

        private async UniTaskVoid UseSinglePatternAsync(
            BasePatternSO pattern,
            Action onAttackEnd,
            CancellationTokenSource cts)
        {
            bool shouldNotifyEnd = false;

            try
            {
                await pattern.UsePattern(_enemy, cts.Token);
                shouldNotifyEnd = !cts.IsCancellationRequested;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(EnemyAttackCompo)}] Pattern attack failed. pattern={pattern.name}", this);
                Debug.LogException(exception, this);
                shouldNotifyEnd = true;
            }
            finally
            {
                if (_singlePatternCts == cts)
                {
                    _singlePatternCts = null;
                    IsSinglePatternRunning = false;
                }

                cts.Dispose();

                if (shouldNotifyEnd)
                    onAttackEnd?.Invoke();
            }
        }

        private void CancelSinglePattern()
        {
            if (_singlePatternCts == null) return;

            _singlePatternCts.Cancel();
            _singlePatternCts = null;
            IsSinglePatternRunning = false;
        }
    }
}
