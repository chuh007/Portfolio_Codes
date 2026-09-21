using System;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.WeaponCombine;
using Chuh007Lib.Bus;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingCombinationView
    {
        private readonly VisualElement _root;
        private readonly CraftingBuildContext _context;
        private readonly Action _refresh;

        public CraftingCombinationView(VisualElement root, CraftingBuildContext context, Action refresh)
        {
            _root = root;
            _context = context;
            _refresh = refresh;
        }

        public void Place(WeaponType type)
        {
            if (!_context.CanPlace(type)) return;
            _context.Placed.Add(type);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.CraftingInstrumentPlaced, SoundType.SFX));
            _refresh();
        }

        public void Clear()
        {
            _context.Placed.Clear();
            _refresh();
        }

        public void Load(CombineMakeDataSO recipe)
        {
            if (_context.Combinations == null || !_context.Combinations.CanCombine(recipe)) return;
            _context.Placed.Clear();
            _context.Placed.AddRange(recipe.needWeapons);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.CraftingInstrumentPlaced, SoundType.SFX));
            _refresh();
        }

        public void Refresh()
        {
            VisualElement row = _root.Q("craft-slot-row");
            row.Clear();
            for (int i = 0; i < CraftingBuildContext.InstrumentOrder.Length; i++)
            {
                VisualElement slot = Box(row, "craft-slot");
                if (i >= _context.Placed.Count)
                {
                    Text(slot, "+", "slot-plus");
                    continue;
                }
                WeaponType type = _context.Placed[i];
                slot.AddToClassList("craft-slot--filled");
                slot.tooltip = "클릭하면 선택에서 제외합니다.";
                slot.focusable = true;
                _context.ApplyIcon(Box(slot, "craft-slot-icon"), type);
                Text(slot, InstrumentPartRules.GetInstrumentDisplayName(type), "craft-slot-label");
                slot.RegisterCallback<ClickEvent>(_ => Remove(type));
                slot.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode != UnityEngine.KeyCode.Return && evt.keyCode != UnityEngine.KeyCode.Space) return;
                    Remove(type);
                    evt.StopPropagation();
                });
            }
            RefreshPreview();
        }

        private void Remove(WeaponType type)
        {
            _context.Placed.Remove(type);
            _refresh();
        }

        private void RefreshPreview()
        {
            CombineMakeDataSO recipe = _context.FindSelectedRecipe();
            bool ready = recipe != null && _context.Combinations.CanCombine(recipe);
            bool owned = recipe != null && _context.Combinations.HasCombinedWeapon(recipe.combineWeapon);
            _root.Q<Button>("combine-button").SetEnabled(ready);
            _root.Q<Button>("clear-combination-button").SetEnabled(_context.Placed.Count > 0);
            string title = recipe != null ? BandName(recipe.combineWeapon) :
                _context.Placed.Count < 2 ? "완성한 악기를 2개 이상 선택하세요" : "일치하는 레시피가 없어요";
            string description = recipe != null ? recipe.description :
                "아래 조합 가이드에서 필요한 악기와 강화 상태를 확인할 수 있어요.";
            if (owned) description = "이미 보유한 밴드입니다. 다른 조합을 선택해 주세요.";
            else if (recipe != null && !ready) description = "재료의 보유 및 강화 상태를 다시 확인해 주세요.";
            _root.Q<Label>("combine-preview-name").text = title;
            _root.Q<Label>("combine-preview-desc").text = description;
        }

        public CombineMakeDataSO Combine()
        {
            CombineMakeDataSO recipe = _context.FindSelectedRecipe();
            if (recipe == null || !_context.Combinations.TryCombine(recipe)) return null;
            _context.Placed.Clear();
            _context.Bind(_context.Combinations);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.CraftingSucceeded, SoundType.SFX));
            _refresh();
            return recipe;
        }
    }
}
