using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingInventoryView
    {
        private readonly VisualElement _root;
        private readonly VisualElement _list;
        private readonly CraftingBuildContext _context;
        private readonly CraftingInstrumentDrag _drag;
        private readonly CraftingCombinedInventory _combined;

        public CraftingInventoryView(VisualElement root, CraftingBuildContext context, CraftingInstrumentDrag drag)
        {
            _root = root;
            _list = root.Q("inventory-row");
            _context = context;
            _drag = drag;
            _combined = new CraftingCombinedInventory(BandName);
        }

        public void Refresh(bool animate)
        {
            _drag.Cancel();
            _list.Clear();
            int owned = 0;
            int complete = 0;
            foreach (WeaponType type in CraftingBuildContext.InstrumentOrder)
            {
                if (!_context.Owns(type)) continue;
                owned++;
                if (_context.IsComplete(type)) complete++;
                AddInstrument(type, animate);
            }
            _combined.Refresh(_root.Q("combined-row"), _context.Combinations);
            int bands = _context.Combinations != null ? _context.Combinations.CreationOrder.Count : 0;
            _root.Q<Label>("instrument-count").text = $"{owned}종";
            _root.Q<Label>("build-summary").text = $"강화 완료 {complete}  /  밴드 {bands}";
            _root.Q("inventory-empty").style.display = owned == 0 && bands == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _root.Q("combined-heading").style.display = bands > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void AddInstrument(WeaponType type, bool animate)
        {
            VisualElement item = Box(_list, "inventory-item");
            item.name = $"instrument-{type}";
            item.EnableInClassList("inventory-item--selected", _context.Selected == type);
            item.EnableInClassList("inventory-item--placed", _context.Placed.Contains(type));
            item.EnableInClassList("inventory-item--complete", _context.IsComplete(type));
            _context.ApplyIcon(Box(item, "inventory-icon"), type);
            VisualElement copy = Box(item, "inventory-copy");
            VisualElement heading = Box(copy, "inventory-topline");
            Text(heading, InstrumentPartRules.GetInstrumentDisplayName(type), "inventory-label");
            Label state = Text(heading, _context.InstrumentState(type), "state-tag");
            state.EnableInClassList("state-tag--ready", _context.IsComplete(type));
            int count = _context.PartCount(type);
            Progress(copy, (float)count / InstrumentPartRules.RequiredPartCount, animate);
            Text(copy, $"강화 파츠  {count} / {InstrumentPartRules.RequiredPartCount}", "inventory-progress-text");
            _drag.Bind(item, type);
        }
    }
}
