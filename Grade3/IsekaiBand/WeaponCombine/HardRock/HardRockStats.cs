using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class HardRockStats
    {
        private const float CooldownMultiplier = 0.5f;
        public float RockDamage = 19.8f;
        public float RockSpeed = 7f;
        public float RockRange = 10f;
        public float ShockwaveDamage = 11f;
        public float ShockwaveRadius = 1.6f;
        public float FragmentDamage = 6.6f;
        public float FragmentSpeed = 11f;
        public float FragmentRange = 4f;

        public void Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, PlayerAttackBase source)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out var guitar))
            {
                float damage = guitar.GetBuildStat(AttackStatName.Damage, RockDamage);
                RockDamage = damage * 1.375f;
                FragmentDamage = damage * 0.715f;
                RockSpeed = guitar.GetBuildStat(AttackStatName.ProjectileSpeed, RockSpeed) * 0.9f;
                RockRange = guitar.GetBuildStat(AttackStatName.Range, RockRange) * 1.35f;
                float cooldown = guitar.GetBuildStat(AttackStatName.Cooldown, 0.8f) * 1.35f;
                float tunedCooldown = CombineWeaponTuning.Interval(
                    cooldown,
                    ingredients.Count,
                    0.55f);
                source.SetRuntimeStat(
                    AttackStatName.Cooldown,
                    source,
                    tunedCooldown * CooldownMultiplier);
            }

            if (ingredients.TryGetValue(WeaponType.Bass, out var bass))
            {
                float damage = bass.GetBuildStat(AttackStatName.Damage, ShockwaveDamage);
                RockDamage += damage * 0.715f;
                ShockwaveDamage = damage * 0.88f;
                float range = bass.GetBuildStat(AttackStatName.Range, RockRange);
                RockRange = Mathf.Max(RockRange, range * 1.2f);
                ShockwaveRadius = Mathf.Max(1.4f, range * 0.3f);
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out var drum))
            {
                RockDamage += drum.GetBuildStat(AttackStatName.Damage, RockDamage) * 0.55f;
                RockRange = Mathf.Max(
                    RockRange,
                    drum.GetBuildStat(AttackStatName.Range, RockRange) * 1.15f);
            }

            FragmentSpeed = Mathf.Max(10f, RockSpeed * 1.5f);
            FragmentRange = Mathf.Max(3.5f, RockRange * 0.45f);

            int ingredientCount = ingredients.Count;
            RockDamage = CombineWeaponTuning.Damage(RockDamage, CombineWeaponType.HardRockBand, ingredientCount);
            ShockwaveDamage = CombineWeaponTuning.Damage(ShockwaveDamage, CombineWeaponType.HardRockBand, ingredientCount);
            FragmentDamage = CombineWeaponTuning.Damage(FragmentDamage, CombineWeaponType.HardRockBand, ingredientCount);
            RockRange = CombineWeaponTuning.Range(RockRange, ingredientCount);
            ShockwaveRadius = CombineWeaponTuning.Range(ShockwaveRadius, ingredientCount);
            FragmentRange = CombineWeaponTuning.Range(FragmentRange, ingredientCount);
        }
    }
}
