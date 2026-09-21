using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class RhythmSectionStats
    {
        private const float BassZoneRangeMultiplier = 0.58f;
        private const float DrumZoneRangeMultiplier = 0.72f;
        private const float DamageMultiplier = 0.8f;
        private const float BassKnockbackMultiplier = 1.5f;
        public float Damage = 15.4f;
        public float Radius = 3.8f;
        public float Knockback = BassAttackBuild.HardCaseKnockback * BassKnockbackMultiplier;

        public float Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            float cooldown = 1f;
            if (ingredients.TryGetValue(WeaponType.Bass, out var bass))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(bass);
                Damage = stats.Damage * 1.76f;
                Radius = stats.Range * BassZoneRangeMultiplier;
                cooldown = stats.Cooldown * 0.65f;
                Knockback = stats.Knockback * BassKnockbackMultiplier;
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out var drum))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(drum);
                Damage += stats.Damage * 0.55f;
                Radius = Mathf.Max(Radius, stats.Range * DrumZoneRangeMultiplier);
            }

            int ingredientCount = ingredients.Count;
            Damage = CombineWeaponTuning.Damage(Damage, CombineWeaponType.RhythmSection, ingredientCount) * DamageMultiplier;
            Radius = CombineWeaponTuning.Range(Radius, ingredientCount);
            cooldown = CombineWeaponTuning.Interval(cooldown, ingredientCount, 0.08f);
            return cooldown;
        }
    }
}
