using UnityEngine;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    public static class MetaUpgradeRuntime
    {
        private const float MinMultiplier = 0.01f;

        public static float GetValue(Component source, MetaUpgradeType type)
        {
            return GetValue(source != null ? source.transform : null, type);
        }

        public static float GetValue(Transform source, MetaUpgradeType type)
        {
            if (source == null) return 0f;

            PlayerStatCompo stats = source.GetComponentInParent<PlayerStatCompo>();
            if (stats == null)
                stats = source.GetComponentInChildren<PlayerStatCompo>(true);
            if (stats == null)
            {
                Entity owner = source.GetComponentInParent<Entity>();
                if (owner != null)
                    stats = owner.GetComponentInChildren<PlayerStatCompo>(true);
            }

            return stats != null ? stats.GetUpgradeValue(type) : 0f;
        }

        public static float GetMultiplier(Component source, MetaUpgradeType type)
        {
            return Mathf.Max(MinMultiplier, 1f + GetValue(source, type));
        }

        public static float GetMultiplier(Transform source, MetaUpgradeType type)
        {
            return Mathf.Max(MinMultiplier, 1f + GetValue(source, type));
        }

        public static float GetCooldownDivisor(Component source)
        {
            return Mathf.Max(MinMultiplier, 1f + GetValue(source, MetaUpgradeType.Cooldown));
        }

        public static float GetCooldownDivisor(Transform source)
        {
            return Mathf.Max(MinMultiplier, 1f + GetValue(source, MetaUpgradeType.Cooldown));
        }
    }
}
