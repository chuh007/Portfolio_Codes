using _Code.LCH._02.Scripts.Card;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine.UIElements;
using static _Work.CHUH.Code.UI.CraftingViewElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingInstrumentDetails
    {
        private readonly VisualElement _root;
        private readonly Button _addButton;
        private readonly CraftingBuildContext _context;

        public CraftingInstrumentDetails(VisualElement root, CraftingBuildContext context)
        {
            _root = root.Q<ScrollView>("instrument-detail");
            _addButton = root.Q<Button>("add-instrument-button");
            _context = context;
        }

        public void Refresh(bool animate)
        {
            _root.Clear();
            WeaponType type = _context.Selected;
            bool owned = _context.Owns(type);
            _addButton.SetEnabled(_context.CanPlace(type));
            _addButton.text = _context.Placed.Contains(type) ? "이미 조합에 선택한 악기" :
                _context.IsComplete(type) ? "이 악기를 조합에 추가" : "파츠를 모두 모으면 조합할 수 있어요";
            if (!owned)
            {
                Text(_root, "보유 악기를 선택해 주세요.", "empty-message");
                return;
            }

            VisualElement hero = Box(_root, "detail-hero");
            _context.ApplyIcon(Box(hero, "detail-icon"), type);
            VisualElement title = Box(hero, "inventory-copy");
            Text(title, InstrumentPartRules.GetInstrumentDisplayName(type), "detail-name");
            Text(title, _context.InstrumentState(type), "detail-status");
            int count = _context.PartCount(type);
            VisualElement heading = Box(_root, "detail-progress-heading");
            Text(heading, "강화 파츠", "subheading");
            Text(heading, $"{count} / {InstrumentPartRules.RequiredPartCount}", "detail-progress-value");
            Progress(_root, (float)count / InstrumentPartRules.RequiredPartCount, animate, true);
            foreach (InstrumentPartDefinition part in InstrumentPartRules.GetParts(type))
                AddPart(part);
            AddEvolution(type);
        }

        private void AddPart(InstrumentPartDefinition part)
        {
            bool owned = _context.Build.HasInstrumentPart(part.WeaponType, part.PartIndex);
            VisualElement row = Box(_root, "part-row");
            row.EnableInClassList("part-row--owned", owned);
            Text(row, owned ? "✓" : "·", "part-marker");
            VisualElement copy = Box(row, "part-copy");
            VisualElement heading = Box(copy, "part-heading");
            Text(heading, part.PartName, "part-name");
            Text(heading, owned ? "획득" : "미획득", "part-state");
            Text(copy, part.Description, "part-description");
        }

        private void AddEvolution(WeaponType type)
        {
            if (!InstrumentEvolutionRules.TryGet(type, out var evolution)) return;
            bool evolved = _context.Build.IsInstrumentEvolved(type);
            bool complete = _context.IsComplete(type);
            bool hasPassive = _context.Passives != null
                && _context.Passives.GetLevel(evolution.RequiredCommonBuildType) > 0;
            var passive = RuntimeCommonBuildCatalog.GetData(evolution.RequiredCommonBuildType);
            string passiveName = passive != null ? passive.displayName : evolution.EvolutionName;
            VisualElement box = Box(_root, "evolution-box");
            Text(box, evolved ? "진화 완료" : "진화 조건", "subheading");
            AddRequirement(box, $"악기 강화 {_context.PartCount(type)} / {InstrumentPartRules.RequiredPartCount}", complete);
            AddRequirement(box, $"{passiveName} 패시브 {(hasPassive ? "보유" : "필요")}", hasPassive);
            Text(box, evolution.Description, "part-description");
            string note = evolved ? "진화 효과가 적용되어 있습니다." :
                _context.Build.CanEvolveInstrument(type) ? "조건 충족! 레벨업 때 진화 선택지가 나올 수 있어요." :
                "두 조건을 채우면 레벨업 때 진화 선택지가 열려요.";
            Text(box, note, "evolution-note");
        }

        private static void AddRequirement(VisualElement parent, string text, bool met)
        {
            Label label = Text(parent, $"{(met ? "✓" : "○")}  {text}", "requirement");
            label.EnableInClassList("requirement--met", met);
        }
    }
}
