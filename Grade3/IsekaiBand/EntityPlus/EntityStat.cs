using System.Collections.Generic;
using System.Linq;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus
{
    public class EntityStat : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private StatOverride[] statOverrides;
        private Dictionary<string, StatSO> _stats; //real stat
        
        public Entity Owner { get; private set; }
        
        public virtual void Initialize(Entity entity)
        {
            Owner = entity;
            ResetStats();
        }

        public void ResetStats()
        {
            //스탯들을 복제하고 오버라이드해서 다시 저장해준다.
            ReleaseStats();
            _stats = new Dictionary<string, StatSO>(statOverrides.Length);
            foreach (StatOverride statOverride in statOverrides)
            {
                StatSO stat = statOverride.CreateStat();
                _stats.Add(stat.statName, stat);
            }
        }

        public void ResetPooledStats()
        {
            if (!CanReuseStats())
            {
                ResetStats();
                return;
            }

            foreach (StatOverride statOverride in statOverrides)
            {
                StatSO stat = _stats[statOverride.StatName];
                stat.ResetRuntimeState(statOverride.Stat);
                if (statOverride.IsUseOverride)
                    stat.BaseValue = statOverride.OverrideValue;
            }
        }

        private bool CanReuseStats()
        {
            if (_stats == null || _stats.Count != statOverrides.Length)
                return false;

            for (int i = 0; i < statOverrides.Length; i++)
            {
                StatOverride statOverride = statOverrides[i];
                //추가 직렬화 필드가 있는 파생 스탯은 원래의 복제 경로를 사용한다.
                if (statOverride.Stat == null
                    || statOverride.Stat.GetType() != typeof(StatSO)
                    || !_stats.TryGetValue(statOverride.StatName, out StatSO stat)
                    || stat == null
                    || stat.GetType() != typeof(StatSO))
                    return false;

                for (int previous = 0; previous < i; previous++)
                {
                    if (statOverrides[previous].StatName == statOverride.StatName)
                        return false;
                }
            }

            return true;
        }

        protected virtual void OnDestroy()
        {
            ReleaseStats();
        }

        private void ReleaseStats()
        {
            if (_stats == null) return;

            foreach (StatSO stat in _stats.Values)
            {
                if (stat != null)
                    Destroy(stat);
            }

            _stats.Clear();
        }

        public StatSO GetStat(StatSO targetStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");
            return _stats[targetStat.statName];
        }

        public bool TryGetStat(StatSO targetStat, out StatSO outStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");

            if (targetStat == null)
            {
                outStat = null;
                return false;
            }

            return _stats.TryGetValue(targetStat.statName, out outStat);
        }
        
        public bool TryGetStatByName(string statName, out StatSO outStat)
        {
            outStat = null;
            return _stats != null && !string.IsNullOrEmpty(statName) && _stats.TryGetValue(statName, out outStat);
        }

        public float GetValueByName(string statName, float defaultValue = 1f)
        {
            return _stats != null && _stats.TryGetValue(statName, out var stat) ? stat.Value : defaultValue;
        }

        public IEnumerable<StatSO> GetAllStats()
        {
            return _stats != null ? _stats.Values : Enumerable.Empty<StatSO>();
        }
        
        public void SetBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue = value;
        public float GetBaseValue(StatSO stat) => GetStat(stat).BaseValue;
        public void IncreaseBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue += value;
        public void AddModifier(StatSO stat, object key, float value) => GetStat(stat).AddValueModifier(key, value);
        
        public void AddPercentModifier(StatSO stat, object key, float value) => GetStat(stat).AddPercentModifier(key, value);
        
        public bool AddValueModifierByName(string statName, object key, float value)
        {
            if (!_stats.TryGetValue(statName, out var stat)) return false;
            stat.AddValueModifier(key, value);
            return true;
        }

        public bool AddPercentModifierByName(string statName, object key, float percent)
        {
            if (!_stats.TryGetValue(statName, out var stat)) return false;
            stat.AddPercentModifier(key, percent);
            return true;
        }

        public void RemoveModifierByKey(object key)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveModifier(key);
                stat.RemovePercentModifier(key);
            }
        }

        public void RemoveModifier(StatSO stat, object key) => GetStat(stat).RemoveModifier(key);
        
        public void RemovePercentModifier(StatSO stat, object key) => GetStat(stat).RemovePercentModifier(key);
        
        public void CleanAllModifier()
        {
            foreach (StatSO stat in _stats.Values)
            {
                stat.ClearModifier();
            }
        }
    }
}
