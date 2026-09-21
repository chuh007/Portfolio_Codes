using System;
using System.Linq;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingRecipeList
    {
        private readonly CraftingBuildContext _context;
        private readonly Action<CombineMakeDataSO> _load;

        public CraftingRecipeList(CraftingBuildContext context, Action<CombineMakeDataSO> load)
        {
            _context = context;
            _load = load;
        }

        public void Populate(ScrollView list, bool compact)
        {
            list.Clear();
            var controller = _context.Combinations;
            if (controller == null || controller.GetAllCombinations() == null)
            {
                Text(list, "등록된 레시피가 없습니다.", "empty-message");
                return;
            }

            var recipes = controller.GetAllCombinations().Where(recipe => recipe != null)
                .OrderBy(recipe => controller.HasCombinedWeapon(recipe.combineWeapon) ? 2 :
                    controller.CanCombine(recipe) ? 0 : 1)
                .ThenByDescending(ReadyCount)
                .ThenBy(recipe => recipe.needWeapons != null ? recipe.needWeapons.Count : 0);
            int count = 0;
            foreach (CombineMakeDataSO recipe in recipes)
            {
                AddRecipe(list, recipe, compact);
                count++;
            }
            if (count == 0) Text(list, "등록된 레시피가 없습니다.", "empty-message");
        }

        private int ReadyCount(CombineMakeDataSO recipe)
            => recipe.needWeapons != null ? recipe.needWeapons.Count(_context.IsComplete) : 0;

        private void AddRecipe(ScrollView list, CombineMakeDataSO recipe, bool compact)
        {
            bool owned = _context.Combinations.HasCombinedWeapon(recipe.combineWeapon);
            bool ready = _context.Combinations.CanCombine(recipe);
            VisualElement entry = Box(list, "recipe-entry");
            entry.EnableInClassList("recipe-entry--ready", ready);
            VisualElement heading = Box(entry, "recipe-entry-header");
            Text(heading, BandName(recipe.combineWeapon), "recipe-entry-name");
            Text(heading, owned ? "보유 중" : ready ? "조합 가능" :
                $"준비 {ReadyCount(recipe)}/{recipe.needWeapons?.Count ?? 0}", "recipe-state");
            VisualElement ingredients = Box(entry, "recipe-ingredients");
            if (recipe.needWeapons != null)
                foreach (WeaponType type in recipe.needWeapons)
                    AddIngredient(ingredients, type, owned);

            if (!compact && !string.IsNullOrWhiteSpace(recipe.description))
                Text(entry, recipe.description, "recipe-entry-desc");
            if (!ready) return;
            var button = new Button(() => _load(recipe)) { text = "재료 한 번에 담기" };
            button.AddToClassList("quiet-button");
            button.AddToClassList("recipe-load-button");
            entry.Add(button);
        }

        private void AddIngredient(VisualElement parent, WeaponType type, bool combined)
        {
            VisualElement ingredient = Box(parent, "recipe-ingredient");
            bool ready = _context.IsComplete(type);
            ingredient.EnableInClassList("recipe-ingredient--ready", ready);
            _context.ApplyIcon(Box(ingredient, "recipe-ingredient-icon"), type);
            string status = combined ? "조합 완료" : ready ? "완료" :
                _context.Owns(type) ? $"{_context.PartCount(type)}/{InstrumentPartRules.RequiredPartCount}" :
                _context.InstrumentState(type);
            Text(ingredient, $"{InstrumentPartRules.GetInstrumentDisplayName(type)} · {status}",
                "recipe-ingredient-name");
        }
    }
}
