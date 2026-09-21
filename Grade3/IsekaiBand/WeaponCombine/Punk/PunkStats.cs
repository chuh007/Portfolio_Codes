using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PunkStats
    {
        private const float BaseSpikeInterval = 0.62f;
        public float SpikeInterval = BaseSpikeInterval;
        public float BaseRadius = 4.6f;
        public float SpikeLength = 9f;
        public float Damage = 17.6f;
        public float Knockback = 1.5f;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            float spikeRangeBasis = BaseRadius;
            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
            {
                float bassRange = bass.GetBuildStat(AttackStatName.Range, BaseRadius);
                PrototypeWeaponFullEvolutionStats bassStats =
                    CombineWeaponTuning.FullEvolutionStats(bass);
                BaseRadius = BassPrototypeAttack.CalculateLowZoneRadius(bassStats.Range);
                spikeRangeBasis = bassRange * 1.2f;
                Damage = bass.GetBuildStat(AttackStatName.Damage, Damage) * 1.595f;
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
            {
                Damage += drum.GetBuildStat(AttackStatName.Damage, Damage) * 0.605f;
                Knockback = drum.GetBuildStat(AttackStatName.KnockbackForce, Knockback) * 0.4f;
            }

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
                Damage += vocal.GetBuildStat(AttackStatName.Damage, Damage) * 0.385f;

            SpikeLength = spikeRangeBasis * 2.15f;
            int ingredientCount = ingredients.Count;
            Damage = CombineWeaponTuning.Damage(Damage, CombineWeaponType.PunkBand, ingredientCount);
            SpikeLength = CombineWeaponTuning.Range(SpikeLength, ingredientCount);
            SpikeInterval = CombineWeaponTuning.Interval(
                BaseSpikeInterval,
                ingredientCount,
                0.16f);
        }
    }
}
