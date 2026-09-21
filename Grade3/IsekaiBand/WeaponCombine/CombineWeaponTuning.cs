using UnityEngine;
using _Code.LCH._02.Scripts.Player.Attack;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class CombineWeaponTuning
    {
        private const float GlobalDamageMultiplier = 0.8f;

        public static PrototypeWeaponFullEvolutionStats FullEvolutionStats(PlayerAttackBase attack)
        {
            if (attack is IPrototypeWeaponFullEvolutionStatsProvider provider)
                return provider.GetFullEvolutionStats();

            return new PrototypeWeaponFullEvolutionStats(
                attack != null ? attack.GetBuildStat(AttackStatName.Damage) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.Range) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.Cooldown) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.ProjectileSpeed) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.TickInterval) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.Duration) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.ConeAngle) : 0f,
                attack != null ? attack.GetBuildStat(AttackStatName.KnockbackForce) : 0f);
        }

        public static float Damage(
            float value,
            CombineWeaponType combinationType,
            int ingredientCount)
            => Mathf.Max(0f, value)
               * GlobalDamageMultiplier
               * GetDamageMultiplier(ingredientCount)
               * CombinePatternTuning.GetPatternEfficiencyCorrection(combinationType)
               * GetProgressionMultiplier(ingredientCount)
               * CombinePatternTuning.GetDeliveryDifficultyMultiplier(combinationType)
               * CombineOpportunityCost.GetOpportunityCostCorrection(combinationType)
               * CombinePatternTuning.GetManualBalanceMultiplier(combinationType);

        public static float Range(float value, int ingredientCount)
            => Mathf.Max(0f, value) * GetRangeMultiplier(ingredientCount);

        public static float Interval(float value, int ingredientCount, float minimum = 0.04f)
            => Mathf.Max(minimum, value * GetIntervalMultiplier(ingredientCount));

        private static float GetDamageMultiplier(int ingredientCount)
            => ingredientCount switch
            {
                >= 5 => 2.15f,
                4 => 1.6f,
                3 => 1.45f,
                _ => 1.35f
            };

        private static float GetRangeMultiplier(int ingredientCount)
            => ingredientCount switch
            {
                >= 5 => 1.55f,
                4 => 1.32f,
                3 => 1.24f,
                _ => 1.18f
            };

        private static float GetIntervalMultiplier(int ingredientCount)
            => ingredientCount switch
            {
                >= 5 => 0.48f,
                4 => 0.64f,
                3 => 0.72f,
                _ => 0.8f
            };

        private static float GetProgressionMultiplier(int ingredientCount)
            => ingredientCount switch
            {
                >= 5 => 1.442f,
                4 => 1.4054f,
                3 => 1.35f,
                _ => 1.35f
            };
    }
}
