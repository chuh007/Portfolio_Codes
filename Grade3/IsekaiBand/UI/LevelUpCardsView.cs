using System.Collections.Generic;
using _Code.LCH._02.Scripts.Card;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    public class LevelUpCardsView : MonoBehaviour
    {
        private static readonly string[] RaritySuffixes = { "common", "rare", "epic", "legendary" };

        [SerializeField] private UIDocument document;
        [SerializeField] private List<UpgradeCardBaseDataSO> previewCards = new();

        private void Awake()
        {
            if (document == null)
                document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            if (previewCards.Count > 0)
                SetCards(previewCards);
        }

        public void SetCards(IReadOnlyList<UpgradeCardBaseDataSO> cards)
        {
            if (document == null || cards == null)
                return;

            VisualElement root = document.rootVisualElement;
            if (root == null)
                return;

            for (int i = 0; i < 3; i++)
                SetCard(root, i + 1, i < cards.Count ? cards[i] : null);
        }

        private void SetCard(VisualElement root, int slot, UpgradeCardBaseDataSO card)
        {
            VisualElement cardRoot = root.Q<VisualElement>($"card-{slot}");
            if (cardRoot == null)
                return;

            cardRoot.style.display = card == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (card == null)
                return;

            Label title = root.Q<Label>($"card-title-{slot}");
            Label desc = root.Q<Label>($"card-desc-{slot}");
            VisualElement icon = root.Q<VisualElement>($"card-icon-{slot}");

            if (title != null)
                title.text = card.cardName;
            if (desc != null)
                desc.text = card.description;
            if (icon != null && card.iconSprite != null)
                icon.style.backgroundImage = new StyleBackground(card.iconSprite);

            ApplyRarity(cardRoot, card.rarity);
        }

        private void ApplyRarity(VisualElement cardRoot, CardRarity rarity)
        {
            string suffix = GetRaritySuffix(rarity);

            Label rarityLabel = cardRoot.Q<Label>(className: "card-rarity");
            VisualElement icon = cardRoot.Q<VisualElement>(className: "card-icon");
            VisualElement rule = cardRoot.Q<VisualElement>(className: "card-rule");
            VisualElement more = cardRoot.Q<VisualElement>(className: "card-more");

            ReplaceRarityClass(cardRoot, "card", suffix);
            ReplaceRarityClass(rarityLabel, "rarity", suffix);
            ReplaceRarityClass(icon, "icon", suffix);
            ReplaceRarityClass(rule, "rule", suffix);
            ReplaceRarityClass(more, "more", suffix);

            if (rarityLabel != null)
                rarityLabel.text = GetRarityText(rarity);
        }

        private static void ReplaceRarityClass(VisualElement element, string prefix, string suffix)
        {
            if (element == null)
                return;

            foreach (string raritySuffix in RaritySuffixes)
                element.RemoveFromClassList($"{prefix}--{raritySuffix}");

            element.AddToClassList($"{prefix}--{suffix}");
        }

        private static string GetRaritySuffix(CardRarity rarity)
            => rarity switch
            {
                CardRarity.Rare => "rare",
                CardRarity.Epic => "epic",
                CardRarity.Legendary => "legendary",
                _ => "common"
            };

        private static string GetRarityText(CardRarity rarity)
            => rarity switch
            {
                CardRarity.Rare => "RARE",
                CardRarity.Epic => "EPIC",
                CardRarity.Legendary => "LEGENDARY",
                _ => "COMMON"
            };
    }
}
