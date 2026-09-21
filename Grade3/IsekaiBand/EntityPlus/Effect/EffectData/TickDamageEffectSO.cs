using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.EntityPlus.Effect;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "SO/Effect/DamageEffect", order = 0)]
    public class TickDamageEffectSO : AbstractEffectDataSO, ITickableEffect
    {
        public float damage;
        public DamageType damageType = DamageType.Physical;
        public float damageDelay;
        [Tooltip("스택당 추가 데미지 배율 (0.2 = 스택당 +20%)")]
        public float damagePerStackMultiplier = 0f;

        public float TickDelay => damageDelay;

        public void OnTick(Entity target)
        {
            IEffectHandler handler = target.GetCompo<EntityEffectController>();
            int stack = 1;
            if (handler is EntityEffectController ctrl)
                stack = Mathf.Max(1, ctrl.GetStackCount(this));

            float finalDamage = damage * (1f + (stack - 1) * damagePerStackMultiplier);
            target.TakeDamage(DamageData.Effect(finalDamage, damageType));
        }
    }
}
