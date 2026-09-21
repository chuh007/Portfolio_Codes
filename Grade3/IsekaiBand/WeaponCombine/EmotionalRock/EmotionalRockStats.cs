using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.EmotionalRock
{
    internal sealed class EmotionalRockStats
    {
        private const float BasePushRadius = 2.45f;
        public float GuitarDamage = 9.9f;
        public float KeyboardDamage = 8.8f;
        public float BassDamage = 13.2f;
        public float VocalDamage = 11f;
        public float ProjectileSpeed = 13f;
        public float ProjectileRange = 10f;
        public float VocalRange = 6f;
        public float PushRadius = BasePushRadius;
        public float PushDamage = 6.6f;

        public void Configure(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Guitar, out PlayerAttackBase guitar))
            {
                GuitarDamage = guitar.GetBuildStat(AttackStatName.Damage, GuitarDamage) * 1.1f;
                ProjectileSpeed = guitar.GetBuildStat(AttackStatName.ProjectileSpeed, ProjectileSpeed);
                ProjectileRange = guitar.GetBuildStat(AttackStatName.Range, ProjectileRange) * 1.35f;
            }

            if (ingredients.TryGetValue(WeaponType.Keyboard, out PlayerAttackBase keyboard))
            {
                KeyboardDamage = keyboard.GetBuildStat(AttackStatName.Damage, KeyboardDamage) * 1.1f;
                ProjectileRange = Mathf.Max(
                    ProjectileRange,
                    keyboard.GetBuildStat(AttackStatName.Range, ProjectileRange) * 1.35f);
            }

            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
                BassDamage = bass.GetBuildStat(AttackStatName.Damage, BassDamage) * 1.21f;

            if (ingredients.TryGetValue(WeaponType.Vocal, out PlayerAttackBase vocal))
            {
                VocalDamage = vocal.GetBuildStat(AttackStatName.Damage, VocalDamage) * 1.1f;
                VocalRange = vocal.GetBuildStat(AttackStatName.Range, VocalRange) * 1.45f;
                PushDamage = vocal.GetBuildStat(AttackStatName.Damage, PushDamage) * 0.495f;
            }

            int ingredientCount = ingredients.Count;
            GuitarDamage = CombineWeaponTuning.Damage(GuitarDamage, CombineWeaponType.EmotionalRockBand, ingredientCount);
            KeyboardDamage = CombineWeaponTuning.Damage(KeyboardDamage, CombineWeaponType.EmotionalRockBand, ingredientCount);
            BassDamage = CombineWeaponTuning.Damage(BassDamage, CombineWeaponType.EmotionalRockBand, ingredientCount);
            VocalDamage = CombineWeaponTuning.Damage(VocalDamage, CombineWeaponType.EmotionalRockBand, ingredientCount);
            PushDamage = CombineWeaponTuning.Damage(PushDamage, CombineWeaponType.EmotionalRockBand, ingredientCount);
            ProjectileRange = CombineWeaponTuning.Range(ProjectileRange, ingredientCount);
            VocalRange = CombineWeaponTuning.Range(VocalRange, ingredientCount);
            PushRadius = CombineWeaponTuning.Range(BasePushRadius, ingredientCount);
        }
    }
}
