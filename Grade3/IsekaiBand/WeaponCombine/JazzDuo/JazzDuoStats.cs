using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzDuoStats
    {
        private const float ProjectileIntervalMultiplier = 1.35f;
        private const float TrailZoneDamageMultiplier = 0.625f;
        public float NoteDamage = 9.9f;
        public float NoteSpeed = 9f;
        public float NoteRange = 7f;
        public float ZoneTickDamage = 6.1875f;
        public float ZoneRadius = 2.2f;

        public void Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, PlayerAttackBase source)
        {
            if (ingredients.TryGetValue(WeaponType.Keyboard, out var piano))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(piano);
                NoteDamage = stats.Damage * 1.32f;
                NoteSpeed = stats.ProjectileSpeed;
                NoteRange = stats.Range;
                float cooldown = CombineWeaponTuning.Interval(
                    stats.Cooldown,
                    ingredients.Count,
                    0.06f) * ProjectileIntervalMultiplier;
                source.SetRuntimeStat(AttackStatName.Cooldown, source, cooldown);
            }

            int ingredientCount = ingredients.Count;
            NoteDamage = CombineWeaponTuning.Damage(NoteDamage, CombineWeaponType.JazzDuo, ingredientCount);
            ZoneTickDamage = NoteDamage * TrailZoneDamageMultiplier;
            NoteRange = CombineWeaponTuning.Range(NoteRange, ingredientCount);
            ZoneRadius = CombineWeaponTuning.Range(ZoneRadius, ingredientCount) * 0.5f;
        }
    }
}
