using _Code.LCH._02.Scripts.Card.Build;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    public static class DebuffEffects
    {
        public static bool IsBoss(Entity target)
        {
            return target != null && ((target is Enemy enemy && enemy.IsBoss)
                || target.CompareTag("Boss") || target.GetType().Name.Contains("Boss"));
        }

        public static bool IsElite(Entity target)
        {
            return target != null && (target.gameObject.tag == "Elite" || target.GetType().Name.Contains("Elite"));
        }

        public static int Count(EntityEffectController controller)
        {
            if (controller == null) return 0;
            int types = 0;
            int count = 0;
            foreach (EntityEffect effect in controller.ActiveEffects)
            {
                if (effect.IsExpired || !(effect is DebuffEffect debuff)) continue;
                int flag = 1 << (int)debuff.Type;
                if ((types & flag) != 0) continue;
                types |= flag;
                count++;
            }
            return count;
        }

        public static DebuffEffect Get(EntityEffectController controller, DebuffType type)
        {
            if (controller == null) return null;
            foreach (EntityEffect effect in controller.ActiveEffects)
                if (!effect.IsExpired && effect is DebuffEffect debuff && debuff.Type == type)
                    return debuff;
            return null;
        }

        public static float GetValue(EntityEffectController controller, DebuffType type)
        {
            if (controller == null) return 0f;
            float value = 0f;
            foreach (EntityEffect effect in controller.ActiveEffects)
                if (!effect.IsExpired && effect is DebuffEffect debuff && debuff.Type == type)
                    value += debuff.Value * debuff.CurrentStack;
            return value;
        }
    }
}
