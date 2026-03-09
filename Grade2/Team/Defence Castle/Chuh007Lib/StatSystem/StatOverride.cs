using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chuh007Lib.StatSystem
{
    [Serializable]
    public class StatOverride
    {
        [field:FormerlySerializedAs("stat")] [field:SerializeField] public StatSO Stat{get ;private set;} 
        [field:FormerlySerializedAs("isUseOverride")] [field:SerializeField] public bool IsUseOverride{get ;private set;}     
        [field:FormerlySerializedAs("overrideValue")] [field:SerializeField] public float OverrideValue{get ;private set;} 
        
        public string StatName => Stat.statName;
        public StatOverride(StatSO stat) => this.Stat = stat; //생성자

        //기본 에셋인 SO 를 클론해서 오버라이드 하거나 기본값으로 만들어주는 매서드
        public StatSO CreateStat()
        {
            StatSO newStat = Stat.Clone() as StatSO; //클론 만들어야 한다.
            Debug.Assert(newStat != null, $"{nameof(newStat)} stat cloning failed");

            if (IsUseOverride)
            {
                newStat.BaseValue = OverrideValue;
            }

            return newStat;
        }

    }
}