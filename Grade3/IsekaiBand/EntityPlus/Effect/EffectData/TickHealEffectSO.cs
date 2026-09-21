using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    [CreateAssetMenu(fileName = "HealEffect", menuName = "SO/Effect/HealEffect", order = 0)]
    public class TickHealEffectSO : AbstractEffectDataSO, ITickableEffect
    {
        public float heal;
        public float healDelay;

        public float TickDelay => healDelay;

        public void OnTick(Entity target) => target.TakeHeal(heal);
    }
}
