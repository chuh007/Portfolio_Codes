using System;
using System.Collections.Generic;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingCombinedInventory
    {
        private readonly Func<CombineWeaponType, string> _getName;
        private readonly List<VisualElement> _items = new();

        public CraftingCombinedInventory(Func<CombineWeaponType, string> getName)
        {
            _getName = getName;
        }

        public void Refresh(VisualElement inventoryRow, CombineWeaponController controller)
        {
            foreach (VisualElement item in _items)
                item.RemoveFromHierarchy();
            _items.Clear();

            if (inventoryRow == null || controller == null) return;

            var displayed = new HashSet<CombineWeaponType>();
            foreach (CombineMakeDataSO recipe in controller.GetAllCombinations())
            {
                if (recipe == null || !controller.HasCombinedWeapon(recipe.combineWeapon)
                    || !displayed.Add(recipe.combineWeapon)) continue;

                VisualElement item = CreateItem(recipe);
                inventoryRow.Add(item);
                _items.Add(item);
            }
        }

        private VisualElement CreateItem(CombineMakeDataSO recipe)
        {
            string displayName = _getName(recipe.combineWeapon);
            // 기본 악기 슬롯의 입력 콜백을 등록하지 않아 재료로 드래그할 수 없다.
            var item = new VisualElement
            {
                name = $"combined-item-{recipe.combineWeapon}",
                tooltip = $"{displayName}\n보유 중인 조합 악기입니다. 재료로 사용할 수 없습니다."
            };
            item.AddToClassList("inventory-item");
            item.AddToClassList("inventory-item--combined");

            var icon = new VisualElement { pickingMode = PickingMode.Ignore };
            icon.AddToClassList("inventory-icon");
            if (recipe.icon != null)
                icon.style.backgroundImage = new StyleBackground(recipe.icon);
            item.Add(icon);

            var label = new Label(displayName) { pickingMode = PickingMode.Ignore };
            label.AddToClassList("inventory-label");
            item.Add(label);

            var ownedLabel = new Label("보유 중") { pickingMode = PickingMode.Ignore };
            ownedLabel.AddToClassList("inventory-owned-label");
            item.Add(ownedLabel);
            return item;
        }
    }
}
