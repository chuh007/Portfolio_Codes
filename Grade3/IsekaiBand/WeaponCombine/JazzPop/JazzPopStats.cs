using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzPopStats
    {
        private const float BaseOutwardDuration = 0.8f;
        public float ExplosionDamage = 26.4f;
        public float ExplosionRadius = 2.8f;
        public float OutwardDistance = 5.2f;
        public float OrbitRadius = 4.5f;
        public float OutwardDuration = BaseOutwardDuration;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, float venueRadius,
            System.Func<float, float> applySpeedDuration)
        {
            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                ExplosionDamage = keyboard.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 1.98f;
                OutwardDistance = keyboard.GetBuildStat(AttackStatName.Range, OutwardDistance) * 0.7f;
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
                ExplosionDamage += drum.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 0.77f;

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
            {
                ExplosionDamage += vocal.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 0.495f;
                ExplosionRadius = Mathf.Max(
                    ExplosionRadius,
                    vocal.GetBuildStat(AttackStatName.Range, ExplosionRadius) * 0.55f);
            }

            OrbitRadius = Mathf.Min(venueRadius * 0.75f, Mathf.Max(3.6f, OutwardDistance));

            int ingredientCount = ingredients.Count;
            ExplosionDamage = CombineWeaponTuning.Damage(ExplosionDamage, CombineWeaponType.JazzPopBand, ingredientCount);
            ExplosionRadius = CombineWeaponTuning.Range(ExplosionRadius, ingredientCount);
            OutwardDistance = CombineWeaponTuning.Range(OutwardDistance, ingredientCount);
            OrbitRadius = CombineWeaponTuning.Range(OrbitRadius, ingredientCount);
            OutwardDuration = applySpeedDuration(BaseOutwardDuration);
        }
    }
}
