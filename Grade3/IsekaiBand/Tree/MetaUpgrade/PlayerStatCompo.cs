using System;
using System.Collections.Generic;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    [Serializable]
    public struct UpgradeStat
    {
        public MetaUpgradeType type;
        public StatSO stat;
    }
    
    public class PlayerStatCompo : EntityStat, IMetaUpgradeable
    {
        // 모든 업그레이드의 '종류'와 Stat을 연결해줌.
        [SerializeField] private List<UpgradeStat> upgradeStatList = new List<UpgradeStat>();
        
        private Dictionary<MetaUpgradeType, StatSO> _upgradeStats = new Dictionary<MetaUpgradeType, StatSO>();
        private readonly List<(StatSO stat, StatSO.ValueChangeHandler handler)> _subscriptions
            = new List<(StatSO, StatSO.ValueChangeHandler)>();

        public event Action<MetaUpgradeType> OnUpgradeValueChanged;
        public event Action<string, float, float> OnStatChanged;

        public override void Initialize(Entity entity)
        {
            ClearStatSubscriptions();
            base.Initialize(entity);
            SubscribeStatChanges();
            _upgradeStats = new Dictionary<MetaUpgradeType, StatSO>();

            foreach (UpgradeStat upgradeStat in upgradeStatList)
            {
                if (upgradeStat.stat == null) continue;
                if (_upgradeStats.ContainsKey(upgradeStat.type)) continue;

                if (TryGetStatByName(upgradeStat.stat.statName, out StatSO runtimeStat))
                {
                    _upgradeStats.Add(upgradeStat.type, runtimeStat);
                    continue;
                }

                Debug.LogWarning(
                    $"[PlayerStatCompo] Stat override is missing for meta upgrade: {upgradeStat.type}",
                    this);
            }
        }

        protected override void OnDestroy()
        {
            ClearStatSubscriptions();
            base.OnDestroy();
        }

        public bool HasStat(string statName)
        {
            return TryGetStatByName(statName, out _);
        }

        public float GetValue(string statName)
        {
            if (TryGetStatByName(statName, out StatSO stat))
                return stat.Value;

            Debug.LogWarning($"[PlayerStatCompo] Stat not found: {statName}", this);
            return 0f;
        }

        public StatSO GetStat(string statName)
        {
            return TryGetStatByName(statName, out StatSO stat) ? stat : null;
        }
        
        public void ApplyUpgrade(MetaUpgradeType type, int id, float value, bool isPercent)
        {
            if (!_upgradeStats.TryGetValue(type, out StatSO stat))
            {
                Debug.LogWarning($"[PlayerStatCompo] Meta upgrade stat is missing: {type}", this);
                return;
            }

            stat.AddValueModifier(id, NormalizeUpgradeValue(value, isPercent));
            OnUpgradeValueChanged?.Invoke(type);
        }
        
        public void RemoveUpgrade(MetaUpgradeType type, int id, bool isPercent)
        {
            if (!_upgradeStats.TryGetValue(type, out StatSO stat)) return;

            stat.RemoveModifier(id);
            OnUpgradeValueChanged?.Invoke(type);
        }

        public float GetUpgradeValue(MetaUpgradeType type)
        {
            return _upgradeStats.TryGetValue(type, out StatSO stat) ? stat.Value : 0f;
        }

        private static float NormalizeUpgradeValue(float value, bool isPercent)
        {
            if (!isPercent) return value;
            if (value > 2f) return value / 100f;
            return value > 1f ? value - 1f : value;
        }

        private void SubscribeStatChanges()
        {
            foreach (StatSO stat in GetAllStats())
            {
                if (stat == null) continue;

                StatSO.ValueChangeHandler handler = (s, cur, prev) => OnStatChanged?.Invoke(s.statName, cur, prev);
                stat.OnValueChanged += handler;
                _subscriptions.Add((stat, handler));
            }
        }

        private void ClearStatSubscriptions()
        {
            foreach (var (stat, handler) in _subscriptions)
            {
                if (stat != null)
                    stat.OnValueChanged -= handler;
            }

            _subscriptions.Clear();
        }
    }
}
