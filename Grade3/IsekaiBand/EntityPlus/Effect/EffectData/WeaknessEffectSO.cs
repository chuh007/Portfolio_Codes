using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    [CreateAssetMenu(fileName = "WeaknessEffect", menuName = "SO/Effect/WeaknessEffect")]
    public class WeaknessEffectSO : AbstractEffectDataSO, IDurationEffect
    {
        public StatSO targetStat;
        public float damageIncrease = 0.15f;
        public float bossMultiplier = 0.5f;

        public void ActiveEffect(Entity target)
        {
            EntityStat stat = target.GetCompo<EntityStat>();
            float value = damageIncrease * (target is Enemy enemy && enemy.IsBoss ? bossMultiplier : 1f);
            stat.AddPercentModifier(targetStat, this, 1f + Mathf.Clamp(value, 0f, 0.5f));
        }

        public void UnActiveEffect(Entity target)
        {
            EntityStat stat = target.GetCompo<EntityStat>();
            stat.RemovePercentModifier(targetStat, this);
        }
    }
}
