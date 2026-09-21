using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingBenchView : IDisposable
    {
        private readonly VisualElement _root;
        private readonly CraftingBuildContext _context;
        private readonly CraftingInventoryView _inventory;
        private readonly CraftingInstrumentDetails _details;
        private readonly CraftingPassiveList _passives;
        private readonly CraftingRecipeList _recipes;
        private readonly CraftingCombinationView _combination;
        private readonly CraftingInstrumentDrag _drag;
        private readonly List<(Button button, Action action)> _buttons = new();
        private bool _recipeVisible;
        private bool _resultVisible;

        public CraftingBenchView(VisualElement root, CraftingBuildContext context, Action close)
        {
            _root = root;
            _context = context;
            _combination = new CraftingCombinationView(root, context, () => Refresh(false));
            _drag = new CraftingInstrumentDrag(root, context, Select, _combination.Place);
            _inventory = new CraftingInventoryView(root, context, _drag);
            _details = new CraftingInstrumentDetails(root, context);
            _passives = new CraftingPassiveList(root, context);
            _recipes = new CraftingRecipeList(context, LoadRecipe);
            Bind("bench-close-button", close);
            Bind("add-instrument-button", () => _combination.Place(context.Selected));
            Bind("clear-combination-button", _combination.Clear);
            Bind("combine-button", Combine);
            Bind("recipe-button", ShowRecipes);
            Bind("recipe-close-button", HidePopups);
            Bind("result-close-button", HidePopups);
        }

        private void Bind(string name, Action action)
        {
            Button button = _root.Q<Button>(name);
            button.clicked += action;
            _buttons.Add((button, action));
        }

        public void Refresh(bool animate)
        {
            _context.Bind(_context.Combinations);
            _inventory.Refresh(animate);
            _details.Refresh(animate);
            _passives.Refresh();
            _combination.Refresh();
            _recipes.Populate(_root.Q<ScrollView>("recipe-shortlist"), true);
            if (_recipeVisible) _recipes.Populate(_root.Q<ScrollView>("recipe-list"), false);
        }

        private void Select(WeaponType type)
        {
            _context.Selected = type;
            Refresh(false);
            _root.Q<ScrollView>("instrument-detail").scrollOffset = UnityEngine.Vector2.zero;
        }

        public void SetVisible(bool visible)
        {
            _root.EnableInClassList("craft-root--visible", visible);
            if (!visible)
            {
                _drag.Cancel();
                HidePopups();
            }
        }

        private void ShowRecipes()
        {
            HidePopups();
            _recipes.Populate(_root.Q<ScrollView>("recipe-list"), false);
            _root.Q("recipe-popup").style.display = DisplayStyle.Flex;
            _recipeVisible = true;
        }

        private void LoadRecipe(CombineMakeDataSO recipe)
        {
            _combination.Load(recipe);
            HidePopups();
        }

        private void Combine()
        {
            CombineMakeDataSO recipe = _combination.Combine();
            HidePopups();
            _root.Q<Label>(className: "result-badge-text").text = recipe != null ? "합주 준비 완료" : "조합 확인";
            _root.Q<Label>("result-name").text = recipe != null ? BandName(recipe.combineWeapon) : "조합할 수 없어요";
            _root.Q<Label>("result-desc").text = recipe != null ? recipe.description : "재료와 조합 조건을 다시 확인해 주세요.";
            VisualElement icon = _root.Q("result-icon-image");
            icon.style.display = recipe != null && recipe.icon != null ? DisplayStyle.Flex : DisplayStyle.None;
            icon.style.backgroundImage = new StyleBackground(recipe != null ? recipe.icon : null);
            _root.Q("result-popup").style.display = DisplayStyle.Flex;
            _resultVisible = true;
        }

        public bool TryClosePopup()
        {
            if (!_recipeVisible && !_resultVisible) return false;
            HidePopups();
            return true;
        }

        private void HidePopups()
        {
            _recipeVisible = false;
            _resultVisible = false;
            _root.Q("recipe-popup").style.display = DisplayStyle.None;
            _root.Q("result-popup").style.display = DisplayStyle.None;
        }

        public void Dispose()
        {
            _drag.Cancel();
            foreach (var binding in _buttons) binding.button.clicked -= binding.action;
            _buttons.Clear();
        }
    }
}
