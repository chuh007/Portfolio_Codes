using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class BalladStats
    {
        private const float KeyboardProjectileSpeedRatio = 0.65f;
        private const float AttackCooldown = 2.8f;
        private const float BassZoneRadiusRatio = 0.72f;
        public float Damage = 12.1f;
        public float SpiralRadius = 9f;
        public float RadialSpeed = 3.4f;
        public float AngularSpeed = 185f;
        public float HitRadius = 0.32f;
        public float BassZoneDamage = 8.8f;
        public float BassZoneRadius = 4.5f;
        public float BassZoneTickInterval = 0.8f;

        public float Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                Damage = keyboard.GetBuildStat(AttackStatName.Damage, Damage) * 1.32f;
                RadialSpeed = keyboard.GetBuildStat(
                    AttackStatName.ProjectileSpeed,
                    RadialSpeed) * KeyboardProjectileSpeedRatio;
                SpiralRadius = keyboard.GetBuildStat(AttackStatName.Range, SpiralRadius) * 1.35f;
            }

            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
            {
                float bassRange = bass.GetBuildStat(AttackStatName.Range, 6f);
                SpiralRadius = Mathf.Max(
                    SpiralRadius,
                    bassRange * 1.65f);
                BassZoneRadius = Mathf.Max(3f, bassRange * BassZoneRadiusRatio);
                BassZoneDamage = bass.GetBuildStat(AttackStatName.Damage, BassZoneDamage) * 1.1f;
                BassZoneTickInterval = Mathf.Max(
                    0.15f,
                    bass.GetBuildStat(AttackStatName.Cooldown, BassZoneTickInterval));
                Damage += BassZoneDamage * 0.35f;
            }

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
            {
                Damage += vocal.GetBuildStat(AttackStatName.Damage, Damage) * 0.33f;
                HitRadius = Mathf.Max(
                    HitRadius,
                    vocal.GetBuildStat(AttackStatName.Scale, HitRadius) * 0.25f);
            }

            int ingredientCount = ingredients.Count;
            Damage = CombineWeaponTuning.Damage(Damage, CombineWeaponType.BalladBand, ingredientCount);
            BassZoneDamage = CombineWeaponTuning.Damage(BassZoneDamage, CombineWeaponType.BalladBand, ingredientCount);
            SpiralRadius = CombineWeaponTuning.Range(SpiralRadius, ingredientCount);
            BassZoneRadius = CombineWeaponTuning.Range(BassZoneRadius, ingredientCount);
            HitRadius = CombineWeaponTuning.Range(HitRadius, ingredientCount);
            BassZoneTickInterval = CombineWeaponTuning.Interval(
                BassZoneTickInterval,
                ingredientCount,
                0.12f);
            return CombineWeaponTuning.Interval(AttackCooldown, ingredientCount, 0.5f);
        }
    }
}
