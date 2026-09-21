using _Code.LCH._02.Scripts.Card;
using _Code.LCH._02.Scripts.Card.Build;
using UnityEngine;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingPassiveList
    {
        private readonly ScrollView _list;
        private readonly Label _count;
        private readonly CraftingBuildContext _context;

        public CraftingPassiveList(VisualElement root, CraftingBuildContext context)
        {
            _list = root.Q<ScrollView>("passive-list");
            _count = root.Q<Label>("passive-count");
            _context = context;
        }

        public void Refresh()
        {
            _list.Clear();
            int count = 0;
            if (_context.Passives != null)
            {
                foreach (CommonBuildDataSO data in RuntimeCommonBuildCatalog.GetDataList())
                {
                    int level = _context.Passives.GetLevel(data.buildType);
                    if (level <= 0) continue;
                    Add(data.displayName, data.description, data.icon, (int)data.grade, level, data.maxLevel);
                    count++;
                }
            }
            if (_context.Cards != null)
            {
                foreach (var card in _context.Cards.SlotManager.AcquiredCards)
                {
                    if (card.Data is not PassiveCard passive || card.Stack <= 0) continue;
                    Add(passive.cardName, passive.description, passive.icon != null ? passive.icon : passive.iconSprite,
                        (int)passive.rarity, card.Stack, 0);
                    count++;
                }
            }
            _count.text = $"{count}종 보유";
            if (count == 0) Text(_list, "획득한 패시브가 여기에 표시됩니다.", "empty-message");
        }

        private void Add(string name, string description, Sprite icon, int grade, int level, int maxLevel)
        {
            VisualElement entry = Box(_list, "passive-entry");
            VisualElement heading = Box(entry, "passive-heading");
            if (icon != null) Box(heading, "passive-icon").style.backgroundImage = new StyleBackground(icon);
            Text(heading, name, "passive-name");
            Text(heading, Grade(grade), "passive-grade");
            Text(heading, maxLevel > 0 ? $"Lv.{level}/{maxLevel}" : $"Lv.{level}", "passive-level");
            Text(entry, description, "passive-description");
        }
    }
}
