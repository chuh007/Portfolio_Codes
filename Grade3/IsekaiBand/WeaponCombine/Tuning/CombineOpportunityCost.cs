using _Code.LCH._02.Scripts.Player.WeaponStyle;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class CombineOpportunityCost
    {
        private const float OpportunityCostTargetMultiplier = 1.5f;

        // 재료 무기의 최대 강화 DPS 합을 기준으로 조합의 기회비용을 보정한다.
        public static float GetOpportunityCostCorrection(CombineWeaponType type)
        {
            if (type == CombineWeaponType.RockStarDuo)
                return 1f;

            float currentEquivalentDps = GetCurrentEquivalentDps(type);
            if (currentEquivalentDps <= 0f)
                return 1f;

            float targetEquivalentDps = GetIngredientOpportunityCost(type)
                                        * OpportunityCostTargetMultiplier;
            return targetEquivalentDps > 0f
                ? targetEquivalentDps / currentEquivalentDps
                : 1f;
        }

        private static float GetIngredientOpportunityCost(CombineWeaponType type)
        {
            var requiredWeapons = CombineWeaponFactory.GetRequiredWeapons(type);
            if (requiredWeapons == null)
                return 0f;

            float total = 0f;
            for (int i = 0; i < requiredWeapons.Count; i++)
                total += GetFullUpgradeEquivalentDps(requiredWeapons[i]);
            return total;
        }

        private static float GetFullUpgradeEquivalentDps(WeaponType type)
            => type switch
            {
                WeaponType.Bass => 27.5f,
                WeaponType.Guitar => 130.4f,
                WeaponType.Drum => 68.6f,
                WeaponType.Keyboard => 165f,
                WeaponType.Vocal => 100f,
                _ => 0f
            };

        private static float GetCurrentEquivalentDps(CombineWeaponType type)
            => type switch
            {
                CombineWeaponType.EmotionalDuo => 1112.126f,
                CombineWeaponType.RhythmSection => 793.809f,
                CombineWeaponType.JazzDuo => 685.877f,

                CombineWeaponType.HardRockBand => 1404.007f,
                CombineWeaponType.JazzBand => 1166.396f,
                CombineWeaponType.PopRockBand => 1667.938f,
                CombineWeaponType.RockBand => 1401.844f,
                CombineWeaponType.BalladBand => 1295.999f,
                CombineWeaponType.PunkBand => 1451.52f,
                CombineWeaponType.SymphonicRock => 1358.528f,

                CombineWeaponType.OrthodoxRockBand => 2356.389f,
                CombineWeaponType.JazzPopBand => 2266.63f,
                CombineWeaponType.FusionJazzBand => 1978.806f,
                CombineWeaponType.EmotionalRockBand => 1750.748f,
                CombineWeaponType.FullBand => 4393.127f,
                _ => 0f
            };
    }
}
