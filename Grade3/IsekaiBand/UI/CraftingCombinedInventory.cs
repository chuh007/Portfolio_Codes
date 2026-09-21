using System;
using System.Collections.Generic;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingCombinedInventory
    {
        private readonly Func<CombineWeaponType, string> _getName;

        public CraftingCombinedInventory(Func<CombineWeaponType, string> getName) => _getName = getName;

        public void Refresh(VisualElement inventoryRow, CombineWeaponController controller)
        {
            if (inventoryRow == null) return;
            inventoryRow.Clear();
            if (controller == null) return;
            var displayed = new HashSet<CombineWeaponType>();
            foreach (CombineMakeDataSO recipe in controller.GetAllCombinations())
            {
                if (recipe == null || !controller.HasCombinedWeapon(recipe.combineWeapon)
                    || !displayed.Add(recipe.combineWeapon)) continue;
                VisualElement item = CraftingViewElements.Box(inventoryRow, "inventory-item");
                item.AddToClassList("inventory-item--combined");
                item.name = $"combined-item-{recipe.combineWeapon}";
                VisualElement icon = CraftingViewElements.Box(item, "inventory-icon");
                if (recipe.icon != null) icon.style.backgroundImage = new StyleBackground(recipe.icon);
                VisualElement copy = CraftingViewElements.Box(item, "inventory-copy");
                CraftingViewElements.Text(copy, _getName(recipe.combineWeapon), "inventory-label");
                CraftingViewElements.Text(copy, "보유 중 · 조합 재료로 사용할 수 없음", "inventory-progress-text");
                CraftingViewElements.Text(copy, recipe.description, "passive-description");
            }
        }
    }
}
