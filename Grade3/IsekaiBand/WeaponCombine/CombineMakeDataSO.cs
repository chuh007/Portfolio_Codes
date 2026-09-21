using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public enum CombineWeaponType
    {
        EmotionalDuo,
        RhythmSection,
        JazzDuo,
        RockStarDuo,
        OrthodoxRockBand,
        HardRockBand,
        JazzBand,
        PopRockBand,
        RockBand,
        BalladBand,
        PunkBand,
        SymphonicRock,
        JazzPopBand,
        FusionJazzBand,
        EmotionalRockBand,
        FullBand
    }
    
    // 필요 무기와 조합 무기 연결하는 녀석.
    [CreateAssetMenu(fileName = "FILENAME", menuName = "SO/WeaponCombine/CombineData", order = 0)]
    public class CombineMakeDataSO : ScriptableObject
    {
        public List<WeaponType> needWeapons;
        public CombineWeaponType combineWeapon;
        public Sprite icon;
        [TextArea] public string description;
    }
}
