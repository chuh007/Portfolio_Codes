using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class EmotionalDuoStats
    {
        private const float BaseNoteInterval = 0.3f;
        public float VocalDamage = 11f;
        public float VocalRange = 4f;
        public float ConeAngle = 55f;
        public float VocalTickInterval = 0.2f;
        public float NoteDamage = 8.8f;
        public float NoteSpeed = 9f;
        public float NoteInterval = BaseNoteInterval;

        public void Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Vocal, out var vocal))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(vocal);
                VocalDamage = stats.Damage * 1.375f;
                VocalRange = stats.Range;
                ConeAngle = stats.ConeAngle > 0f ? stats.ConeAngle : ConeAngle;
                VocalTickInterval = stats.TickInterval > 0f
                    ? stats.TickInterval
                    : VocalTickInterval;
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out var piano))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(piano);
                NoteDamage = stats.Damage * 1.32f;
                NoteSpeed = stats.ProjectileSpeed;
                NoteInterval = Mathf.Min(BaseNoteInterval, stats.Cooldown);
            }

            int ingredientCount = ingredients.Count;
            VocalDamage = CombineWeaponTuning.Damage(VocalDamage, CombineWeaponType.EmotionalDuo, ingredientCount);
            NoteDamage = CombineWeaponTuning.Damage(NoteDamage, CombineWeaponType.EmotionalDuo, ingredientCount);
            VocalRange = CombineWeaponTuning.Range(VocalRange, ingredientCount);
            VocalTickInterval = CombineWeaponTuning.Interval(VocalTickInterval, ingredientCount);
            NoteInterval = CombineWeaponTuning.Interval(NoteInterval, ingredientCount);
        }
    }
}
