using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class OrthodoxRockStats
    {
        public float VenueRadius = 6f;
        public float VenueDamagePerSecond = 8.8f;
        public float SlowMultiplier = 0.65f;
        public float RockDamage = 22f;
        public float RockRange = 7f;
        public float RockSpeed = 9f;
        public float LightningDamage = 24.2f;
        public float LightningRadius = 1.15f;
        public float ShockwaveDamage = 17.6f;
        public float ShockwaveKnockback = 5f;

        public void Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Vocal, out var vocal))
            {
                RockDamage = vocal.GetBuildStat(AttackStatName.Damage, RockDamage) * 1.375f;
                RockRange = vocal.GetBuildStat(AttackStatName.Range, RockRange) * 1.6f;
            }

            if (ingredients.TryGetValue(WeaponType.Guitar, out var guitar))
            {
                LightningDamage = guitar.GetBuildStat(AttackStatName.Damage, LightningDamage) * 1.595f;
                RockDamage += guitar.GetBuildStat(AttackStatName.Damage, RockDamage) * 0.77f;
                RockSpeed = guitar.GetBuildStat(AttackStatName.ProjectileSpeed, RockSpeed) * 0.8f;
                VenueRadius = Mathf.Max(VenueRadius, guitar.GetBuildStat(AttackStatName.Range, VenueRadius));
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out var drum))
            {
                ShockwaveDamage = drum.GetBuildStat(AttackStatName.Damage, ShockwaveDamage) * 1.375f;
                ShockwaveKnockback = drum.GetBuildStat(AttackStatName.KnockbackForce, ShockwaveKnockback);
                VenueRadius = Mathf.Max(VenueRadius, drum.GetBuildStat(AttackStatName.Range, VenueRadius));
            }

            if (ingredients.TryGetValue(WeaponType.Bass, out var bass))
            {
                float bassDamage = bass.GetBuildStat(AttackStatName.Damage, 8f);
                VenueDamagePerSecond = bass.GetBuildStat(AttackStatName.DotDamagePerSec, bassDamage * 0.4f) * 1.1f;
                SlowMultiplier = bass.GetBuildStat(AttackStatName.SlowMultiplier, SlowMultiplier);
                VenueRadius = Mathf.Max(VenueRadius, bass.GetBuildStat(AttackStatName.Range, VenueRadius));
            }

            int ingredientCount = ingredients.Count;
            VenueDamagePerSecond = CombineWeaponTuning.Damage(VenueDamagePerSecond, CombineWeaponType.OrthodoxRockBand, ingredientCount);
            RockDamage = CombineWeaponTuning.Damage(RockDamage, CombineWeaponType.OrthodoxRockBand, ingredientCount);
            LightningDamage = CombineWeaponTuning.Damage(LightningDamage, CombineWeaponType.OrthodoxRockBand, ingredientCount);
            ShockwaveDamage = CombineWeaponTuning.Damage(ShockwaveDamage, CombineWeaponType.OrthodoxRockBand, ingredientCount);
            VenueRadius = CombineWeaponTuning.Range(VenueRadius * 1.15f, ingredientCount);
            RockRange = CombineWeaponTuning.Range(RockRange, ingredientCount);
            RockSpeed *= 1.15f;
            LightningRadius = Mathf.Max(0.9f, VenueRadius * 0.17f);
        }
    }
}
