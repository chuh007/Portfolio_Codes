using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class FusionJazzStats
    {
        public float ProjectileDamage = 9.9f;
        public float ProjectileSpeed = 16f;
        public float TargetRange = 10f;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, float venueRadius)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
            {
                ProjectileDamage = guitar.GetBuildStat(AttackStatName.Damage, ProjectileDamage) * 0.99f;
                ProjectileSpeed = guitar.GetBuildStat(AttackStatName.ProjectileSpeed, ProjectileSpeed) * 1.2f;
                TargetRange = guitar.GetBuildStat(AttackStatName.Range, TargetRange) * 1.55f;
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                ProjectileDamage += keyboard.GetBuildStat(AttackStatName.Damage, ProjectileDamage) * 0.55f;
                TargetRange = Mathf.Max(
                    TargetRange,
                    keyboard.GetBuildStat(AttackStatName.Range, TargetRange) * 1.45f);
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
                ProjectileDamage += drum.GetBuildStat(AttackStatName.Damage, ProjectileDamage) * 0.385f;

            TargetRange = Mathf.Max(TargetRange, venueRadius * 1.15f);
            int ingredientCount = ingredients.Count;
            ProjectileDamage = CombineWeaponTuning.Damage(ProjectileDamage, CombineWeaponType.FusionJazzBand, ingredientCount);
            TargetRange = CombineWeaponTuning.Range(TargetRange, ingredientCount);
            ProjectileSpeed *= 1.18f;
        }
    }
}
