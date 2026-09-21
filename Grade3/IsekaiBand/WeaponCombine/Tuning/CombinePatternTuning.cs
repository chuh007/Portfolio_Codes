namespace _Work.CHUH.Code.WeaponCombine
{
    // 패턴의 명중 효율과 운용 난이도에 따른 피해 보정 계수다.
    internal static class CombinePatternTuning
    {
        public static float GetPatternEfficiencyCorrection(CombineWeaponType type)
            => type switch
            {
                // Baseline calibration for 2-instrument patterns.
                CombineWeaponType.EmotionalDuo => 0.56743f,
                CombineWeaponType.RhythmSection => 6.02373f,
                CombineWeaponType.JazzDuo => 0.43852f,
                CombineWeaponType.RockStarDuo => 1f,

                // Baseline calibration for 3-instrument patterns.
                CombineWeaponType.HardRockBand => 1.11940f,
                CombineWeaponType.JazzBand => 1.40515f,
                CombineWeaponType.PopRockBand => 1.41650f,
                CombineWeaponType.RockBand => 2.23246f,
                CombineWeaponType.BalladBand => 0.69251f,
                CombineWeaponType.PunkBand => 1.62639f,
                CombineWeaponType.SymphonicRock => 3.22227f,

                // Baseline calibration for 4-instrument patterns.
                CombineWeaponType.OrthodoxRockBand => 9.39392f,
                CombineWeaponType.JazzPopBand => 6.03757f,
                CombineWeaponType.FusionJazzBand => 2.51141f,
                CombineWeaponType.EmotionalRockBand => 13.60853f,

                // Baseline calibration for the extreme 5-instrument projectile density.
                CombineWeaponType.FullBand => 0.447f,
                _ => 1f
            };

        public static float GetDeliveryDifficultyMultiplier(CombineWeaponType type)
            => type switch
            {
                // Directional cone/random notes, small radius, and narrow trail zones.
                CombineWeaponType.EmotionalDuo => 1.15f,
                CombineWeaponType.RhythmSection => 1.05f,
                CombineWeaponType.JazzDuo => 1.30f,
                CombineWeaponType.RockStarDuo => 1f,

                // Targeted/wide patterns are reliable; scatter, thin radial, and capped
                // projectiles receive a miss-risk premium.
                CombineWeaponType.HardRockBand => 1f,
                CombineWeaponType.JazzBand => 0.90f,
                CombineWeaponType.PopRockBand => 1.08f,
                CombineWeaponType.RockBand => 1.18f,
                CombineWeaponType.BalladBand => 0.80f,
                CombineWeaponType.PunkBand => 1.12f,
                CombineWeaponType.SymphonicRock => 1.40f,

                // Venue attacks with automatic target acquisition are more reliable than
                // spring/radial attacks or patterns with venue downtime.
                CombineWeaponType.OrthodoxRockBand => 1.18f,
                CombineWeaponType.JazzPopBand => 1.12f,
                CombineWeaponType.FusionJazzBand => 0.88f,
                CombineWeaponType.EmotionalRockBand => 0.92f,
                CombineWeaponType.FullBand => 1f,
                _ => 1f
            };

        public static float GetManualBalanceMultiplier(CombineWeaponType type)
            => type switch
            {
                CombineWeaponType.EmotionalDuo => 1.32f,
                // 풀강 기본 타격(0.4264초)과 강화 충격파(7초)의 합산 DPS 기준.
                CombineWeaponType.RhythmSection => 0.77189082f,
                CombineWeaponType.JazzDuo => 1.20f,
                CombineWeaponType.HardRockBand => 1.44f,
                CombineWeaponType.JazzBand => 1.20f,
                CombineWeaponType.PopRockBand => 1.20f,
                CombineWeaponType.RockBand => 1.32f,
                CombineWeaponType.BalladBand => 1.20f,
                CombineWeaponType.PunkBand => 1.80f,
                CombineWeaponType.SymphonicRock => 1.20f,
                CombineWeaponType.OrthodoxRockBand => 1.20f,
                CombineWeaponType.JazzPopBand => 1.20f,
                CombineWeaponType.FusionJazzBand => 1.20f,
                CombineWeaponType.EmotionalRockBand => 1.20f,
                CombineWeaponType.FullBand => 1.20f,
                _ => 1f
            };
    }
}
