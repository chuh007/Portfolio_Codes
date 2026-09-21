using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.WeaponStyle;

namespace _Work.CHUH.Code.WeaponCombine
{
    public static class CombineWeaponFactory
    {
        private static readonly WeaponType[] EmotionalDuoWeapons = { WeaponType.Vocal, WeaponType.Keyboard };
        private static readonly WeaponType[] RhythmSectionWeapons = { WeaponType.Drum, WeaponType.Bass };
        private static readonly WeaponType[] JazzDuoWeapons = { WeaponType.Keyboard, WeaponType.Bass };
        private static readonly WeaponType[] RockStarDuoWeapons = { WeaponType.Guitar, WeaponType.Vocal };
        private static readonly WeaponType[] OrthodoxRockBandWeapons =
            { WeaponType.Guitar, WeaponType.Drum, WeaponType.Bass, WeaponType.Vocal };
        private static readonly WeaponType[] HardRockBandWeapons =
            { WeaponType.Guitar, WeaponType.Bass, WeaponType.Drum };
        private static readonly WeaponType[] JazzBandWeapons =
            { WeaponType.Drum, WeaponType.Keyboard, WeaponType.Bass };
        private static readonly WeaponType[] PopRockBandWeapons =
            { WeaponType.Guitar, WeaponType.Keyboard, WeaponType.Vocal };
        private static readonly WeaponType[] RockBandWeapons =
            { WeaponType.Guitar, WeaponType.Drum, WeaponType.Vocal };
        private static readonly WeaponType[] BalladBandWeapons =
            { WeaponType.Bass, WeaponType.Keyboard, WeaponType.Vocal };
        private static readonly WeaponType[] PunkBandWeapons =
            { WeaponType.Drum, WeaponType.Bass, WeaponType.Vocal };
        private static readonly WeaponType[] SymphonicRockWeapons =
            { WeaponType.Guitar, WeaponType.Keyboard, WeaponType.Drum };
        private static readonly WeaponType[] JazzPopBandWeapons =
            { WeaponType.Drum, WeaponType.Bass, WeaponType.Keyboard, WeaponType.Vocal };
        private static readonly WeaponType[] FusionJazzBandWeapons =
            { WeaponType.Guitar, WeaponType.Drum, WeaponType.Bass, WeaponType.Keyboard };
        private static readonly WeaponType[] EmotionalRockBandWeapons =
            { WeaponType.Guitar, WeaponType.Keyboard, WeaponType.Bass, WeaponType.Vocal };
        private static readonly WeaponType[] FullBandWeapons =
            { WeaponType.Bass, WeaponType.Guitar, WeaponType.Drum, WeaponType.Keyboard, WeaponType.Vocal };

        public static CombineWeaponBase Create(CombineWeaponType type)
        {
            return type switch
            {
                CombineWeaponType.EmotionalDuo => new EmotionalDuoAttack(),
                CombineWeaponType.RhythmSection => new RhythmSectionAttack(),
                CombineWeaponType.JazzDuo => new JazzDuoAttack(),
                CombineWeaponType.RockStarDuo => new RockStarDuoAttack(),
                CombineWeaponType.OrthodoxRockBand => new OrthodoxRockBandAttack(),
                CombineWeaponType.HardRockBand => new HardRockBandAttack(),
                CombineWeaponType.JazzBand => new JazzBandAttack(),
                CombineWeaponType.PopRockBand => new PopRockBandAttack(),
                CombineWeaponType.RockBand => new RockBandAttack(),
                CombineWeaponType.BalladBand => new BalladBandAttack(),
                CombineWeaponType.PunkBand => new PunkBandAttack(),
                CombineWeaponType.SymphonicRock => new SymphonicRockAttack(),
                CombineWeaponType.JazzPopBand => new JazzPopBandAttack(),
                CombineWeaponType.FusionJazzBand => new FusionJazzBandAttack(),
                CombineWeaponType.EmotionalRockBand => new EmotionalRockBandAttack(),
                CombineWeaponType.FullBand => new FullBandAttack(),
                _ => null
            };
        }

        public static bool MatchesRecipe(CombineWeaponType type, IReadOnlyList<WeaponType> weapons)
        {
            IReadOnlyList<WeaponType> required = GetRequiredWeapons(type);

            if (required == null || weapons == null || required.Count != weapons.Count)
                return false;

            foreach (WeaponType requiredWeapon in required)
            {
                bool found = false;
                foreach (WeaponType weapon in weapons)
                {
                    if (weapon != requiredWeapon) continue;
                    found = true;
                    break;
                }
                if (!found) return false;
            }
            return true;
        }

        internal static IReadOnlyList<WeaponType> GetRequiredWeapons(CombineWeaponType type)
        {
            return type switch
            {
                CombineWeaponType.EmotionalDuo => EmotionalDuoWeapons,
                CombineWeaponType.RhythmSection => RhythmSectionWeapons,
                CombineWeaponType.JazzDuo => JazzDuoWeapons,
                CombineWeaponType.RockStarDuo => RockStarDuoWeapons,
                CombineWeaponType.OrthodoxRockBand => OrthodoxRockBandWeapons,
                CombineWeaponType.HardRockBand => HardRockBandWeapons,
                CombineWeaponType.JazzBand => JazzBandWeapons,
                CombineWeaponType.PopRockBand => PopRockBandWeapons,
                CombineWeaponType.RockBand => RockBandWeapons,
                CombineWeaponType.BalladBand => BalladBandWeapons,
                CombineWeaponType.PunkBand => PunkBandWeapons,
                CombineWeaponType.SymphonicRock => SymphonicRockWeapons,
                CombineWeaponType.JazzPopBand => JazzPopBandWeapons,
                CombineWeaponType.FusionJazzBand => FusionJazzBandWeapons,
                CombineWeaponType.EmotionalRockBand => EmotionalRockBandWeapons,
                CombineWeaponType.FullBand => FullBandWeapons,
                _ => null
            };
        }
    }
}
