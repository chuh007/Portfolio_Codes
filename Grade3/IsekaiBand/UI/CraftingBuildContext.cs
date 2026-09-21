using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Card;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingBuildContext
    {
        public static readonly WeaponType[] InstrumentOrder =
        {
            WeaponType.Bass, WeaponType.Guitar, WeaponType.Drum, WeaponType.Keyboard, WeaponType.Vocal
        };

        private readonly Func<WeaponType, Sprite> _getIcon;
        public CombineWeaponController Combinations { get; private set; }
        public PlayerAttackCompo Attacks { get; private set; }
        public WeaponBuildManager Build { get; private set; }
        public PlayerCommonBuildCompo Passives { get; private set; }
        public CardSlotManagerCompo Cards { get; private set; }
        public readonly List<WeaponType> Placed = new();
        public WeaponType Selected = WeaponType.None;

        public CraftingBuildContext(Func<WeaponType, Sprite> getIcon) => _getIcon = getIcon;

        public void Bind(CombineWeaponController controller)
        {
            if (Combinations != controller)
            {
                Placed.Clear();
                Selected = WeaponType.None;
            }
            Combinations = controller;
            Attacks = controller != null ? controller.GetComponent<PlayerAttackCompo>() : null;
            Build = Attacks != null ? WeaponBuildManager.GetOrCreate(Attacks) : null;
            Passives = controller != null ? controller.GetComponent<PlayerCommonBuildCompo>() : null;
            Cards = controller != null ? controller.GetComponent<CardSlotManagerCompo>() : null;
            Placed.RemoveAll(type => !IsComplete(type));
            if (Owns(Selected)) return;
            Selected = WeaponType.None;
            foreach (WeaponType type in InstrumentOrder)
            {
                if (!Owns(type)) continue;
                Selected = type;
                break;
            }
        }

        public bool Owns(WeaponType type) => Attacks != null && Attacks.HasWeapon(type);
        public int PartCount(WeaponType type) => Build != null ? Build.GetCollectedPartCount(type) : 0;
        public bool IsComplete(WeaponType type) => Owns(type) && Build != null && Build.IsInstrumentComplete(type);
        public bool CanPlace(WeaponType type) => IsComplete(type) && !Placed.Contains(type);

        public string InstrumentState(WeaponType type)
        {
            if (Build != null && Build.IsInstrumentConsumed(type)) return "조합에 사용됨";
            if (!Owns(type)) return "미보유";
            if (Placed.Contains(type)) return "조합에 선택됨";
            if (Build != null && Build.IsInstrumentEvolved(type)) return "진화 완료";
            return IsComplete(type) ? "강화 완료" : "강화 중";
        }

        public void ApplyIcon(VisualElement element, WeaponType type)
        {
            Sprite icon = _getIcon(type);
            if (icon != null)
                element.style.backgroundImage = new StyleBackground(icon);
            else
                element.style.backgroundColor = InstrumentPartRules.GetInstrumentColor(type);
        }

        public CombineMakeDataSO FindSelectedRecipe()
        {
            if (Combinations == null || Placed.Count < 2) return null;
            foreach (CombineMakeDataSO recipe in Combinations.GetAllCombinations())
            {
                if (recipe == null || recipe.needWeapons == null || recipe.needWeapons.Count != Placed.Count)
                    continue;
                bool matches = true;
                foreach (WeaponType needed in recipe.needWeapons)
                    if (!Placed.Contains(needed)) matches = false;
                if (matches) return recipe;
            }
            return null;
        }
    }
}
