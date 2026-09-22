    using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IAfterInitalize, IHealth
    {
        private const string DamageReceivedMultiplierStatName = "damageReceivedMultiplier";
        private const float EnemyHitVolumeMultiplier = 0.5f;
        private const float EnemyHitPitchMultiplier = 3f;
        private const float EnemyHitSoundInterval = 0.05f;

        [SerializeField] private StatSO hpStat;
        [SerializeField] private StatSO defenseStat;
        [SerializeField, Tooltip("보스는 이 설정과 관계없이 데미지 텍스트를 표시합니다.")]
        private bool showDamageText = false;

        [Header("Boss Damage Balancing")]
        [SerializeField, Min(0.1f)] private float bossDamageWindowDuration = 2f;
        [SerializeField, Range(0.001f, 1f)] private float bossDamageThresholdRatio = 0.075f;
        [SerializeField, Range(0f, 1f)] private float bossDamageAfterThresholdMultiplier = 0.1f;
        [SerializeField, Range(0.001f, 1f)] private float bossLowDamageThresholdRatio = 0.02f;
        [SerializeField, Min(0f)] private float bossLowDamageBoostStep = 0.05f;
        
        public float MaxHealth { get; private set; }
        public float CurrentHealth => _currentHealth;
        private float _currentHealth;
        private float _currentDefense = 0;
        
        private Entity _entity;
        private EntityStat _statCompo;
        private PlayerStatCompo _metaStats;
        private DamageTypeMultiplier _damageTypeMultiplier;
        private IMinimumHealthPolicy _minimumHealthPolicy;
        private IHealthDepletionHandler _healthDepletionHandler;
        private BossDamageAttenuator _bossDamageAttenuator;

        public IHealthDepletionHandler HealthDepletionHandler
        {
            get => _healthDepletionHandler;
            set => _healthDepletionHandler = value;
        }

        private bool ShouldShowDamageText => showDamageText || (_entity is Enemy enemy && enemy.IsBoss);
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = _entity.GetCompo<EntityStat>();
            _metaStats = _entity.GetComponentInChildren<PlayerStatCompo>(true);
            _damageTypeMultiplier = _entity.GetComponentInChildren<DamageTypeMultiplier>(true);
            _bossDamageAttenuator = null;
            MonoBehaviour[] behaviours = _entity.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (_minimumHealthPolicy == null && behaviours[i] is IMinimumHealthPolicy minimumHealthPolicy)
                    _minimumHealthPolicy = minimumHealthPolicy;

                if (_healthDepletionHandler == null && behaviours[i] is IHealthDepletionHandler depletionHandler)
                    _healthDepletionHandler = depletionHandler;

                if (_minimumHealthPolicy != null && _healthDepletionHandler != null)
                    break;
            }
        }
        
        public void AfterInitialize()
        {
            if (_metaStats != null)
                _metaStats.OnUpgradeValueChanged += HandleMetaUpgradeValueChanged;

            _bossDamageAttenuator?.Reset();
            ResetToStatHealth();
        }

        public void ResetToStatHealth(bool preserveCurrentHealthRatio = false)
        {
            float previousMaxHealth = MaxHealth;
            float previousHealthRatio = previousMaxHealth > 0f
                ? Mathf.Clamp01(_currentHealth / previousMaxHealth)
                : 1f;

            float baseMaxHealth = hpStat != null ? hpStat.Value : 0f;
            if (_statCompo != null
                && hpStat != null
                && _statCompo.TryGetStatByName(hpStat.statName, out StatSO hp))
                baseMaxHealth = hp.Value;

            float permanentMaxHealthBonus = Mathf.Max(
                0f,
                MetaUpgradeRuntime.GetValue(this, MetaUpgradeType.MaxHealth));
            MaxHealth = Mathf.Max(0f, baseMaxHealth + permanentMaxHealthBonus);
            _currentHealth = preserveCurrentHealthRatio && previousMaxHealth > 0f
                ? MaxHealth * previousHealthRatio
                : MaxHealth;
            OnHpChanged?.Invoke(_currentHealth);

            RefreshDefense();
        }

        private void RefreshDefense()
        {
            _currentDefense = MetaUpgradeRuntime.GetValue(this, MetaUpgradeType.Defence);
            if (defenseStat == null) return;
            if (_statCompo != null && _statCompo.TryGetStatByName(defenseStat.statName, out StatSO def))
                _currentDefense += def.Value;
        }

        public Action<float> OnHpChanged { get; set; }

        public bool IsBossDamageAttenuationEnabled => _bossDamageAttenuator != null;

        public void SetBossDamageAttenuationEnabled(bool enabled)
        {
            _bossDamageAttenuator?.Reset();
            _bossDamageAttenuator = enabled && _entity is Enemy enemy && enemy.IsBoss
                ? new BossDamageAttenuator(
                    bossDamageWindowDuration,
                    bossDamageThresholdRatio,
                    bossDamageAfterThresholdMultiplier,
                    bossLowDamageThresholdRatio,
                    bossLowDamageBoostStep)
                : null;
        }

        public void TakeDamage(float damage, bool ignoreDefense = false, DamageType damageType = DamageType.Physical, bool isFixedDamage = false)
        {
            if (_entity.IsDead) return;
            float typeMultiplier = !isFixedDamage && _damageTypeMultiplier != null
                ? _damageTypeMultiplier.GetMultiplier(damageType)
                : 1f;
            float receivedMultiplier = !isFixedDamage
                ? _statCompo?.GetValueByName(DamageReceivedMultiplierStatName) ?? 1f
                : 1f;
            float multipliedDamage = damage * typeMultiplier * receivedMultiplier;
            float defense = ignoreDefense || isFixedDamage ? 0f : _currentDefense;
            float finalDamage = isFixedDamage
                ? Mathf.Max(0f, damage)
                : multipliedDamage <= 0f ? 0f : Mathf.Max(multipliedDamage - defense, 1f);
            float damageTimestamp = Time.time;
            float attenuatedDamage = !isFixedDamage && _bossDamageAttenuator != null
                ? _bossDamageAttenuator.Attenuate(finalDamage, MaxHealth, damageTimestamp)
                : finalDamage;
            float minimumHealth = _minimumHealthPolicy != null
                ? Mathf.Clamp(_minimumHealthPolicy.MinimumHealth, 0f, MaxHealth)
                : 0f;
            float previousHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(_currentHealth - attenuatedDamage, minimumHealth, MaxHealth);
            float appliedDamage = Mathf.Max(0f, previousHealth - _currentHealth);
            _bossDamageAttenuator?.Record(appliedDamage, damageTimestamp);

            if (appliedDamage > 0f && _entity is Enemy)
            {
                Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                    SoundKeys.EnemyHit,
                    SoundType.SFX,
                    EnemyHitVolumeMultiplier,
                    EnemyHitPitchMultiplier,
                    suppressDuplicateThisFrame: true,
                    duplicateSuppressionWindowSeconds: EnemyHitSoundInterval));
            }
            
            if (ShouldShowDamageText)
                Bus<EnemyHitEvent>.Raise(new EnemyHitEvent(transform.position, attenuatedDamage, damageType, typeMultiplier));
            
            OnHpChanged?.Invoke(_currentHealth);
            if (_currentHealth <= 0f)
            {
                if (_healthDepletionHandler?.TryHandleHealthDepleted() == true)
                    return;

                _entity.OnDead?.Invoke();
                _entity.IsDead = true;
            }
        }

        public void TakeHeal(float heal)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + heal, 0f, MaxHealth);
            OnHpChanged?.Invoke(_currentHealth);
        }

        public void SetCurrentHealthRatio(float ratio, bool notify = true)
        {
            _currentHealth = MaxHealth * Mathf.Clamp01(ratio);
            if (notify)
                OnHpChanged?.Invoke(_currentHealth);
        }

        private void OnDestroy()
        {
            if (_metaStats != null)
                _metaStats.OnUpgradeValueChanged -= HandleMetaUpgradeValueChanged;
        }

        private void HandleMetaUpgradeValueChanged(MetaUpgradeType type)
        {
            switch (type)
            {
                case MetaUpgradeType.MaxHealth:
                    ResetToStatHealth(true);
                    break;
                case MetaUpgradeType.Defence:
                    RefreshDefense();
                    break;
            }
        }
    }

    internal sealed class BossDamageAttenuator
    {
        private readonly struct DamageSample
        {
            public DamageSample(float timestamp, float damage)
            {
                Timestamp = timestamp;
                Damage = damage;
            }

            public float Timestamp { get; }
            public float Damage { get; }
        }

        private readonly float _windowDuration;
        private readonly float _thresholdRatio;
        private readonly float _afterThresholdMultiplier;
        private readonly float _lowDamageThresholdRatio;
        private readonly float _lowDamageBoostStep;
        private readonly Queue<DamageSample> _samples = new();

        private float _damageInWindow;
        private float _lowDamageWindowStartTime = float.NaN;
        private float _lowDamageInWindow;
        private float _damageReceivedMultiplier = 1f;
        private bool _lowDamageBoostLocked;

        public BossDamageAttenuator(
            float windowDuration,
            float thresholdRatio,
            float afterThresholdMultiplier,
            float lowDamageThresholdRatio,
            float lowDamageBoostStep)
        {
            _windowDuration = Mathf.Max(0.1f, windowDuration);
            _thresholdRatio = Mathf.Clamp(thresholdRatio, 0.001f, 1f);
            _afterThresholdMultiplier = Mathf.Clamp01(afterThresholdMultiplier);
            _lowDamageThresholdRatio = Mathf.Clamp(lowDamageThresholdRatio, 0.001f, 1f);
            _lowDamageBoostStep = Mathf.Max(0f, lowDamageBoostStep);
        }

        public float Attenuate(float incomingDamage, float maxHealth, float timestamp)
        {
            RemoveExpiredSamples(timestamp);
            if (incomingDamage <= 0f || maxHealth <= 0f)
                return 0f;

            EvaluateLowDamageWindows(timestamp, maxHealth);
            float boostedDamage = incomingDamage * _damageReceivedMultiplier;
            float threshold = maxHealth * _thresholdRatio;
            float fullDamageAllowance = Mathf.Max(0f, threshold - _damageInWindow);
            float fullDamage = Mathf.Min(boostedDamage, fullDamageAllowance);
            float attenuatedDamage = boostedDamage - fullDamage;
            return fullDamage + attenuatedDamage * _afterThresholdMultiplier;
        }

        public void Record(float appliedDamage, float timestamp)
        {
            if (appliedDamage <= 0f)
                return;

            _samples.Enqueue(new DamageSample(timestamp, appliedDamage));
            _damageInWindow += appliedDamage;

            if (!_lowDamageBoostLocked)
                _lowDamageInWindow += appliedDamage;
        }

        public void Reset()
        {
            _samples.Clear();
            _damageInWindow = 0f;
            _lowDamageWindowStartTime = float.NaN;
            _lowDamageInWindow = 0f;
            _damageReceivedMultiplier = 1f;
            _lowDamageBoostLocked = false;
        }

        private void EvaluateLowDamageWindows(float timestamp, float maxHealth)
        {
            if (_lowDamageBoostLocked || _lowDamageBoostStep <= 0f)
                return;

            if (float.IsNaN(_lowDamageWindowStartTime))
            {
                _lowDamageWindowStartTime = timestamp;
                return;
            }

            float lowDamageThreshold = maxHealth * _lowDamageThresholdRatio;
            while (timestamp - _lowDamageWindowStartTime >= _windowDuration)
            {
                if (_lowDamageInWindow > lowDamageThreshold)
                {
                    _lowDamageBoostLocked = true;
                    return;
                }

                _damageReceivedMultiplier += _lowDamageBoostStep;
                _lowDamageInWindow = 0f;
                _lowDamageWindowStartTime += _windowDuration;
            }
        }

        private void RemoveExpiredSamples(float timestamp)
        {
            float cutoff = timestamp - _windowDuration;
            while (_samples.Count > 0 && _samples.Peek().Timestamp <= cutoff)
                _damageInWindow -= _samples.Dequeue().Damage;

            _damageInWindow = Mathf.Max(0f, _damageInWindow);
        }
    }
}
