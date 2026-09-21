using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal static class CraftingViewElements
    {
        public static VisualElement Box(VisualElement parent, string className)
        {
            var element = new VisualElement();
            element.AddToClassList(className);
            parent.Add(element);
            return element;
        }

        public static Label Text(VisualElement parent, string text, string className)
        {
            var label = new Label(text);
            label.AddToClassList(className);
            parent.Add(label);
            return label;
        }

        public static void Progress(VisualElement parent, float progress, bool animate, bool large = false)
        {
            VisualElement track = Box(parent, "progress-track");
            track.EnableInClassList("detail-progress-track", large);
            VisualElement fill = Box(track, "inventory-progress-fill");
            float target = Mathf.Clamp01(progress) * 100f;
            if (!animate)
            {
                fill.style.width = Length.Percent(target);
                return;
            }

            // UI 스케줄러와 실제 시간을 사용해 게임이 일시정지된 동안에도 차오른다.
            float start = Time.realtimeSinceStartup;
            IVisualElementScheduledItem animation = null;
            animation = fill.schedule.Execute(() =>
            {
                float t = Mathf.Clamp01((Time.realtimeSinceStartup - start) / 0.55f);
                fill.style.width = Length.Percent(target * (1f - Mathf.Pow(1f - t, 3f)));
                if (t >= 1f) animation.Pause();
            }).Every(16);
        }

        public static string Grade(int grade) => grade switch
        {
            1 => "희귀", 2 => "영웅", 3 => "전설", _ => "일반"
        };

        public static string BandName(CombineWeaponType type) => type switch
        {
            CombineWeaponType.EmotionalDuo => "감성 듀오",
            CombineWeaponType.RhythmSection => "리듬 섹션",
            CombineWeaponType.JazzDuo => "재즈 듀오",
            CombineWeaponType.RockStarDuo => "락스타 듀오",
            CombineWeaponType.OrthodoxRockBand => "정통 록 밴드",
            CombineWeaponType.HardRockBand => "하드 록 밴드",
            CombineWeaponType.JazzBand => "재즈 밴드",
            CombineWeaponType.PopRockBand => "팝 록 밴드",
            CombineWeaponType.RockBand => "락 밴드",
            CombineWeaponType.BalladBand => "발라드 밴드",
            CombineWeaponType.PunkBand => "펑크 밴드",
            CombineWeaponType.SymphonicRock => "심포닉 록",
            CombineWeaponType.JazzPopBand => "재즈 팝 밴드",
            CombineWeaponType.FusionJazzBand => "퓨전 재즈 밴드",
            CombineWeaponType.EmotionalRockBand => "감성 록 밴드",
            CombineWeaponType.FullBand => "완전체 밴드",
            _ => type.ToString()
        };
    }
}
