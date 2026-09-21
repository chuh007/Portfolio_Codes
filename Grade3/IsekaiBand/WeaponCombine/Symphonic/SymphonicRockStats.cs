using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class SymphonicRockStats
    {
        private const float ProjectileSpeedBuffMultiplier = 2f;
        public float Damage = 15.4f;
        public float Speed = 22f;
        public float MaxOwnerDistance = 12f;
        public float BounceRange = 6f;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients, PlayerAttackBase source)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(guitar);
                Damage = stats.Damage * 1.485f;
                Speed = stats.ProjectileSpeed * 1.1f * ProjectileSpeedBuffMultiplier;
                MaxOwnerDistance = stats.Range * 1.5f;
                BounceRange = Mathf.Max(3f, stats.Range * 0.75f);
                source.SetRuntimeStat(
                    AttackStatName.Cooldown,
                    source,
                    CombineWeaponTuning.Interval(
                        stats.Cooldown,
                        ingredients.Count,
                        0.15f));
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(keyboard);
                Damage += stats.Damage * 0.55f;
                Speed = Mathf.Max(
                    Speed,
                    stats.ProjectileSpeed * ProjectileSpeedBuffMultiplier);
                MaxOwnerDistance = Mathf.Max(
                    MaxOwnerDistance,
                    stats.Range * 1.45f);
                BounceRange = Mathf.Max(BounceRange, stats.Range * 0.8f);
            }

            if (ingredients.TryGetValue(WeaponType.Drum, out PlayerAttackBase drum))
            {
                PrototypeWeaponFullEvolutionStats stats =
                    CombineWeaponTuning.FullEvolutionStats(drum);
                Damage += stats.Damage * 0.55f;
                MaxOwnerDistance = Mathf.Max(
                    MaxOwnerDistance,
                    stats.Range * 1.25f);
                BounceRange = Mathf.Max(BounceRange, stats.Range * 0.7f);
            }

            int ingredientCount = ingredients.Count;
            Damage = CombineWeaponTuning.Damage(Damage, CombineWeaponType.SymphonicRock, ingredientCount);
            MaxOwnerDistance = CombineWeaponTuning.Range(
                MaxOwnerDistance,
                ingredientCount);
            BounceRange = CombineWeaponTuning.Range(BounceRange, ingredientCount);
        }
    }
}
