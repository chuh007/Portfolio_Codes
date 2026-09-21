using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PopRockStats
    {
        private const float BaseTravelDistance = 3f;
        private const float BaseDecelerationDuration = 0.8f;
        private const float BaseExplosionRadius = 2f;
        private const float AfterglowDamageMultiplier = 0.1f;
        private const float AfterglowRadiusMultiplier = 0.72f;
        private const float AttackCooldown = 2f;
        public float ExplosionDamage = 17.6f;
        public float AfterglowTickDamage = 1.76f;
        public float TravelDistance = BaseTravelDistance;
        public float ExplosionRadius = BaseExplosionRadius;
        public float AfterglowRadius = BaseExplosionRadius * AfterglowRadiusMultiplier;
        public float DecelerationDuration = BaseDecelerationDuration;

        public float Configure(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients,
            System.Func<float, float> applyRange, System.Func<float, float> applySpeedDuration)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
                ExplosionDamage = guitar.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 2.2f;

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
                ExplosionDamage += keyboard.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 0.605f;

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
                ExplosionDamage += vocal.GetBuildStat(AttackStatName.Damage, ExplosionDamage) * 0.44f;

            int ingredientCount = ingredients.Count;
            ExplosionDamage = CombineWeaponTuning.Damage(ExplosionDamage, CombineWeaponType.PopRockBand, ingredientCount);
            AfterglowTickDamage = ExplosionDamage * AfterglowDamageMultiplier;
            TravelDistance = applyRange(
                CombineWeaponTuning.Range(BaseTravelDistance, ingredientCount));
            ExplosionRadius = applyRange(
                CombineWeaponTuning.Range(BaseExplosionRadius, ingredientCount));
            AfterglowRadius = ExplosionRadius * AfterglowRadiusMultiplier;
            DecelerationDuration = applySpeedDuration(
                BaseDecelerationDuration);
            return CombineWeaponTuning.Interval(AttackCooldown, ingredientCount, 0.5f);
        }
    }
}
