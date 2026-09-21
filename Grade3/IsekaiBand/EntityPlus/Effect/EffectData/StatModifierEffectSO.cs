using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;
using _Work.CHUH.Code.EntityPlus.Effect;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    /// <summary>
    /// Stat 변경시키는 Effect
    /// 슬로우나 뎀감, 뎀증 이런거 여기서 하면 됩니다.
    /// </summary>
    [CreateAssetMenu(fileName = "StatModifierEffect", menuName = "SO/Effect/StatModifierEffect", order = 0)]
    public class StatModifierEffectSO : AbstractEffectDataSO, IDurationEffect
    {
        public enum ModifierType { Value, Percent }

        public StatSO targetStat;
        public float value;
        public ModifierType modifierType;

        public void ActiveEffect(Entity target)
        {
            EntityStat stat = target.GetCompo<EntityStat>();
            if (modifierType == ModifierType.Value)
                stat.AddModifier(targetStat, this, value);
            else
                stat.GetStat(targetStat).AddPercentModifier(this, value);
        }

        public void UnActiveEffect(Entity target)
        {
            EntityStat stat = target.GetCompo<EntityStat>();
            if (modifierType == ModifierType.Value)
                stat.RemoveModifier(targetStat, this);
            else
                stat.GetStat(targetStat).RemovePercentModifier(this);
        }
    }
}
