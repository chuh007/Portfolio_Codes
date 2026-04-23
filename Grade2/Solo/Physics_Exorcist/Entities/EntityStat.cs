using System;
using System.Collections.Generic;
using System.Linq;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _01Scripts.Entities
{
    public class EntityStat : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private StatOverride[] statOverrides;
        private StatSO[] _stats; //real stat
        
        public Entity Owner { get; private set; }
        
        public void Initialize(Entity entity)
        {
            Owner = entity;
            //스탯들을 복제하고 오버라이드해서 다시 저장해준다.
            _stats = statOverrides.Select(stat => stat.CreateStat()).ToArray(); 
        }

        public StatSO GetStat(StatSO targetStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");
            return _stats.FirstOrDefault(stat => stat.statName == targetStat.statName);
        }

        public StatSO GetStat(string statName)
        {
            return _stats.FirstOrDefault(stat => stat.statName == statName);
        }

        public bool TryGetStat(StatSO targetStat, out StatSO outStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");
            
            outStat = _stats.FirstOrDefault(stat => stat.statName == targetStat.statName);
            return outStat;
        }
        
        public void SetBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue = value;
        public float GetBaseValue(StatSO stat) => GetStat(stat).BaseValue;
        public void IncreaseBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue += value;
        public void AddModifier(StatSO stat, object key, float value) => GetStat(stat).AddModifier(key, value);
        public void RemoveModifier(StatSO stat, object key) => GetStat(stat).RemoveModifier(key);

        public void CleanAllModifier()
        {
            foreach (StatSO stat in _stats)
            {
                stat.ClearModifier();
            }
        }
        
        public float SubscribeStat(StatSO stat, StatSO.ValueChangeHandler handler, float defaultValue)
        {
            StatSO target = GetStat(stat);
            if (target == null) return defaultValue;
            target.OnValueChanged += handler;
            return target.Value;
        }

        public void UnSubscribeStat(StatSO stat, StatSO.ValueChangeHandler handler)
        {
            StatSO target = GetStat(stat);
            if (target == null) return;
            target.OnValueChanged -= handler;
        }
        
        
        #region Save logic

        [Serializable]
        public struct StatSaveData
        {
            public string statName;
            public float baseValue;
        }
        
        public List<StatSaveData> GetSaveData()
            => _stats.Aggregate(new List<StatSaveData>(), (saveList, stat) =>
                {
                    saveList.Add(new StatSaveData{ statName = stat.statName, baseValue = stat.BaseValue});
                    return saveList;
                });
        

        public void RestoreData(List<StatSaveData> loadedDataList)
        {
            foreach (StatSaveData loadData in loadedDataList)
            {
                StatSO targetStat = _stats.FirstOrDefault(stat => stat.statName == loadData.statName);
                if (targetStat != default)
                {
                    targetStat.BaseValue = loadData.baseValue;
                }
            }
        }
        #endregion
    }
}