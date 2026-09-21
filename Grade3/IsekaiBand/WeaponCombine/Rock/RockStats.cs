using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class RockStats
    {
        private const float BaseRockTravelDistance = 20f;
        private const float AttackCooldown = 2.4f;
        private const float ProjectileSizeMultiplier = 1.10f;
        public float Damage = 19.8f;
        public float Speed = 9f;
        public float HitRadius = 0.55f;
        public float TravelDistance = BaseRockTravelDistance;

        public float Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, System.Func<float, float> applyRange)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
            {
                Damage = guitar.GetBuildStat(AttackStatName.Damage, Damage) * 1.375f;
                Speed = guitar.GetBuildStat(AttackStatName.ProjectileSpeed, Speed) * 0.82f;
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
            {
                Damage += drum.GetBuildStat(AttackStatName.Damage, Damage) * 0.495f;
                HitRadius = Mathf.Max(
                    HitRadius,
                    drum.GetBuildStat(AttackStatName.Scale, HitRadius) * 0.45f);
            }

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
                Damage += vocal.GetBuildStat(AttackStatName.Damage, Damage) * 0.385f;

            int ingredientCount = ingredients.Count;
            Damage = CombineWeaponTuning.Damage(Damage, CombineWeaponType.RockBand, ingredientCount);
            TravelDistance = applyRange(
                CombineWeaponTuning.Range(BaseRockTravelDistance, ingredientCount));
            HitRadius = CombineWeaponTuning.Range(HitRadius, ingredientCount)
                         * ProjectileSizeMultiplier;
            return CombineWeaponTuning.Interval(AttackCooldown, ingredientCount, 0.5f);
        }
    }
}
