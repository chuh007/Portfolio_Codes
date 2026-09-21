using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal sealed class FullBandStats
    {
        public float VocalDamage = 19.8f;
        public float VocalRange = 7f;
        public float RhythmDamage = 30.8f;
        public float RhythmKnockback = 8f;
        public float PianoDamage = 13.2f;
        public float PianoSpeed = 30f;
        public float PianoRange = 12f;
        public float LightningDamage = 35.2f;
        public float LightningRadius = 1.35f;
        public float LightningTargetRange = 14f;
        public float DrumDamage = 28.6f;
        public float DrumSpeed = 22f;
        public float DrumRange = 12f;
        public float DrumShockwaveRadius = 1.55f;
        public float DrumKnockback = 7f;
        public float VenueRadius;
        public float VenueDamagePerSecond;
        public float VenueSlowMultiplier;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients,
            float venueRadius, float venueDamagePerSecond, float venueSlowMultiplier)
        {
            VenueRadius = venueRadius;
            VenueDamagePerSecond = venueDamagePerSecond;
            VenueSlowMultiplier = venueSlowMultiplier;
            int ingredientCount = ingredients.Count;

            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
            {
                var stats = CombineWeaponTuning.FullEvolutionStats(bass);
                VenueRadius = Mathf.Max(VenueRadius, stats.Range * 1.65f)
                              * 0.8f;
                VenueDamagePerSecond = Mathf.Max(VenueDamagePerSecond, stats.Damage * 2.42f);
                VenueSlowMultiplier = Mathf.Min(VenueSlowMultiplier, 0.48f);
                RhythmDamage = stats.Damage * 3.3f;
                RhythmKnockback = Mathf.Max(RhythmKnockback, stats.Knockback * 2f);
            }

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
            {
                var stats = CombineWeaponTuning.FullEvolutionStats(vocal);
                VocalDamage = stats.Damage * 1.595f;
                VocalRange = Mathf.Max(VocalRange, stats.Range * 1.75f);
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                var stats = CombineWeaponTuning.FullEvolutionStats(keyboard);
                PianoDamage = stats.Damage * 1.21f;
                PianoSpeed = Mathf.Max(PianoSpeed, stats.ProjectileSpeed * 2.8f);
                PianoRange = Mathf.Max(PianoRange, stats.Range * 1.8f);
            }

            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
            {
                var stats = CombineWeaponTuning.FullEvolutionStats(guitar);
                LightningDamage = stats.Damage * 2.42f;
                LightningTargetRange = Mathf.Max(LightningTargetRange, stats.Range * 2f);
                LightningRadius = Mathf.Max(LightningRadius, stats.Range * 0.18f);
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
            {
                var stats = CombineWeaponTuning.FullEvolutionStats(drum);
                DrumDamage = stats.Damage * 1.815f;
                DrumSpeed = Mathf.Max(DrumSpeed, stats.ProjectileSpeed * 1.8f);
                DrumRange = Mathf.Max(DrumRange, stats.Range * 1.65f);
                DrumShockwaveRadius = Mathf.Max(DrumShockwaveRadius, stats.Range * 0.2f);
                DrumKnockback = Mathf.Max(DrumKnockback, stats.Knockback * 1.75f);
            }

            VocalDamage = CombineWeaponTuning.Damage(VocalDamage, CombineWeaponType.FullBand, ingredientCount);
            RhythmDamage = CombineWeaponTuning.Damage(RhythmDamage, CombineWeaponType.FullBand, ingredientCount);
            PianoDamage = CombineWeaponTuning.Damage(PianoDamage, CombineWeaponType.FullBand, ingredientCount);
            LightningDamage = CombineWeaponTuning.Damage(LightningDamage, CombineWeaponType.FullBand, ingredientCount);
            DrumDamage = CombineWeaponTuning.Damage(DrumDamage, CombineWeaponType.FullBand, ingredientCount);
            VocalRange = CombineWeaponTuning.Range(VocalRange, ingredientCount);
            PianoRange = CombineWeaponTuning.Range(PianoRange, ingredientCount);
            LightningTargetRange = CombineWeaponTuning.Range(LightningTargetRange, ingredientCount);
            DrumRange = CombineWeaponTuning.Range(DrumRange, ingredientCount);
            LightningRadius = CombineWeaponTuning.Range(LightningRadius, ingredientCount);
            DrumShockwaveRadius = CombineWeaponTuning.Range(DrumShockwaveRadius, ingredientCount);
        }

        public void SetVenueRadius(float radius)
        {
            VenueRadius = radius;
            VocalRange = Mathf.Max(VocalRange, radius);
            LightningTargetRange = Mathf.Max(LightningTargetRange, radius * 1.35f);
            PianoRange = Mathf.Max(PianoRange, radius * 1.2f);
            DrumRange = Mathf.Max(DrumRange, radius * 1.15f);
        }
    }
}
