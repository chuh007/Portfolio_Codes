using System.Collections.Generic;
using System.Linq;
using ChipmunkKingdoms.Scripts.Utility;
using Chuh007Lib.StatSystem;
using UnityEngine;
using Work.CHUH._01Scripts.Enemies;

namespace Work.CHUH._01Scripts.Entities
{
    public class EntityStat : MonoBehaviour, IContainerComponent
    {
        [SerializeField] public StatOverride[] statOverrides;
        private Dictionary<string, StatSO> _stats = new();
        public ComponentContainer ComponentContainer { get; set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            _stats = new Dictionary<string, StatSO>();
            foreach (StatOverride statOverride in statOverrides)
            {
                StatSO newStat = statOverride.CreateStat();
                _stats.Add(newStat.statName, newStat);
            }
        }

        public StatSO GetStat(StatSO targetStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");
            return _stats.GetValueOrDefault(targetStat.statName);
        }


        public bool TryGetStat(StatSO targetStat, out StatSO outStat)
        {
            Debug.Assert(targetStat != null, "Stats::GetStat : target stat is null");

            outStat = _stats.GetValueOrDefault(targetStat.statName);
            return outStat;
        }

        public void AddStat(string targetStatName, string reason, float value)
        {
            StatSO stat = _stats.GetValueOrDefault(targetStatName);
            value = stat.Value * (value - 1);
            stat.AddValueModifier(reason, value);
        }

        public void ClearAllStatChange()
        {
            foreach (var stat in _stats)
            {
                stat.Value.ClearModifier();
            }
        }
    }
}