using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal class CraftingRecipeList
    {
        private readonly Func<CombineWeaponType, string> _getName;
        private readonly Action<VisualElement, WeaponType> _applyIcon;

        public CraftingRecipeList(Func<CombineWeaponType, string> getName,
            Action<VisualElement, WeaponType> applyIcon)
        {
            _getName = getName;
            _applyIcon = applyIcon;
        }

        public void Populate(ScrollView recipeList, Func<IReadOnlyList<CombineMakeDataSO>> getRecipes)
        {
            recipeList.Clear();

            IReadOnlyList<CombineMakeDataSO> recipes = getRecipes();
            int recipeCount = 0;
            if (recipes != null)
            {
                foreach (CombineMakeDataSO recipe in recipes)
                {
                    if (recipe == null) continue;

                    recipeList.Add(CreateRecipeEntry(recipe));
                    recipeCount++;
                }
            }

            if (recipeCount > 0) return;

            var emptyLabel = new Label("등록된 레시피가 없습니다.");
            emptyLabel.AddToClassList("recipe-empty-label");
            recipeList.Add(emptyLabel);
        }

        private VisualElement CreateRecipeEntry(CombineMakeDataSO recipe)
        {
            var entry = new VisualElement();
            entry.AddToClassList("recipe-entry");

            var nameLabel = new Label(_getName(recipe.combineWeapon));
            nameLabel.AddToClassList("recipe-entry-name");
            entry.Add(nameLabel);

            var ingredients = new VisualElement();
            ingredients.AddToClassList("recipe-ingredients");
            if (recipe.needWeapons != null)
            {
                foreach (WeaponType weaponType in recipe.needWeapons)
                    ingredients.Add(CreateRecipeIngredient(weaponType));
            }
            entry.Add(ingredients);

            if (!string.IsNullOrWhiteSpace(recipe.description))
            {
                var descriptionLabel = new Label(recipe.description);
                descriptionLabel.AddToClassList("recipe-entry-desc");
                entry.Add(descriptionLabel);
            }

            return entry;
        }

        private VisualElement CreateRecipeIngredient(WeaponType weaponType)
        {
            var ingredient = new VisualElement();
            ingredient.AddToClassList("recipe-ingredient");

            var icon = new VisualElement();
            icon.AddToClassList("recipe-ingredient-icon");
            _applyIcon(icon, weaponType);
            ingredient.Add(icon);

            var nameLabel = new Label(InstrumentPartRules.GetInstrumentDisplayName(weaponType));
            nameLabel.AddToClassList("recipe-ingredient-name");
            ingredient.Add(nameLabel);

            return ingredient;
        }

    }
}
