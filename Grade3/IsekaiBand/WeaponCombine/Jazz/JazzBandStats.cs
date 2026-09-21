using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzBandStats
    {
        private const float AttackCooldown = 1.65f;
        public float NoteDamage = 8.8f;
        public float NoteSpeed = 10f;
        public float NoteRange = 8f;
        public float ImpactShockwaveDamage = 7.7f;
        public float ImpactShockwaveRadius = 1.8f;
        public float LaunchShockwaveDamage = 12.1f;
        public float LaunchShockwaveRadius = 3f;
        public float ShockwaveKnockback = 4f;
        public float BassZoneTickDamage = 3.3f;
        public float BassZoneRadius = 1.8f;

        public float Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
            {
                float drumDamage = drum.GetBuildStat(
                    AttackStatName.Damage,
                    ImpactShockwaveDamage);
                float drumRange = drum.GetBuildStat(AttackStatName.Range, 5f);
                ImpactShockwaveDamage = drumDamage * 0.792f;
                LaunchShockwaveDamage = drumDamage * 1.265f;
                ImpactShockwaveRadius = Mathf.Max(1.6f, drumRange * 0.34f);
                ShockwaveKnockback = drum.GetBuildStat(
                    AttackStatName.KnockbackForce,
                    ShockwaveKnockback);
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase piano))
            {
                NoteDamage = piano.GetBuildStat(AttackStatName.Damage, NoteDamage) * 0.858f;
                NoteSpeed = piano.GetBuildStat(AttackStatName.ProjectileSpeed, NoteSpeed) * 1.15f;
                NoteRange = piano.GetBuildStat(AttackStatName.Range, NoteRange) * 1.25f;
            }

            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
            {
                float bassDamage = bass.GetBuildStat(AttackStatName.Damage, 10f);
                float bassRange = bass.GetBuildStat(AttackStatName.Range, 6f);
                BassZoneTickDamage = Mathf.Max(1.1f, bassDamage * 0.33f);
                BassZoneRadius = Mathf.Max(1.5f, bassRange * 0.32f);
            }

            int ingredientCount = ingredients.Count;
            NoteDamage = CombineWeaponTuning.Damage(NoteDamage, CombineWeaponType.JazzBand, ingredientCount);
            ImpactShockwaveDamage = CombineWeaponTuning.Damage(ImpactShockwaveDamage, CombineWeaponType.JazzBand, ingredientCount);
            LaunchShockwaveDamage = CombineWeaponTuning.Damage(LaunchShockwaveDamage, CombineWeaponType.JazzBand, ingredientCount);
            BassZoneTickDamage = CombineWeaponTuning.Damage(BassZoneTickDamage, CombineWeaponType.JazzBand, ingredientCount);
            NoteRange = CombineWeaponTuning.Range(NoteRange, ingredientCount);
            ImpactShockwaveRadius = CombineWeaponTuning.Range(
                ImpactShockwaveRadius,
                ingredientCount);
            LaunchShockwaveRadius = ImpactShockwaveRadius * 1.65f;
            BassZoneRadius = CombineWeaponTuning.Range(BassZoneRadius, ingredientCount);

            return CombineWeaponTuning.Interval(AttackCooldown, ingredientCount, 0.45f);
        }
    }
}
