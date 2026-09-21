using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Card;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI.Chest
{
    [RequireComponent(typeof(UIDocument))]
    public class LootRevealUI : MonoBehaviour
    {
        public event Action OnOpenCompleted;
        public event Action OnCloseRequested;

        private const int  MaxSlots             = 10;
        private const long CardStaggerMs        = 170L;
        private const long RevealStartMs        = 20L;
        private const long TransitionDurationMs = 620L;

        private UIDocument           _document;
        private CardSlotManagerCompo _slotManager;
        private bool _canClose;
        private int  _generation;

        // 캐시하지 않음 — UIDocument가 OnEnable마다 rootVisualElement를 재생성할 수 있음
        private VisualElement Root => _document != null ? _document.rootVisualElement : null;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = Root;
            if (root == null) return;
            root.style.display = DisplayStyle.None;
            root.pickingMode = PickingMode.Ignore;
            root.RegisterCallback<ClickEvent>(OnPanelClicked);
        }

        private void OnDisable()
        {
            Root?.UnregisterCallback<ClickEvent>(OnPanelClicked);
        }

        private void OnPanelClicked(ClickEvent _)
        {
            if (!_canClose) return;
            OnCloseRequested?.Invoke();
        }

        // ─── 외부 인터페이스 ────────────────────────────────────────────

        public void OpenChestUpgradeUI(int cnt)
        {
            if (_slotManager == null)
                _slotManager = FindFirstObjectByType<CardSlotManagerCompo>();

            if (_slotManager == null)
            {
                Debug.LogWarning("[LootRevealUI] CardSlotManagerCompo를 찾을 수 없습니다.");
                OnOpenCompleted?.Invoke();
                return;
            }

            var root = Root;
            if (root == null)
            {
                Debug.LogWarning("[LootRevealUI] rootVisualElement이 null입니다. UIDocument 설정을 확인하세요.");
                OnOpenCompleted?.Invoke();
                return;
            }

            _canClose = false;

            var results   = _slotManager.LastChestUpgradeResults;
            int showCount = Mathf.Clamp(cnt, 0, Mathf.Min(MaxSlots, results.Count));

            SetupCards(root, showCount, results);

            root.pickingMode = PickingMode.Position;
            root.style.display = DisplayStyle.Flex;

            PlayRevealAnimations(root, showCount);
        }

        public void Close()
        {
            var root = Root;
            if (root == null) return;

            _generation++;
            _canClose = false;
            root.style.display = DisplayStyle.None;
            root.pickingMode = PickingMode.Ignore;
            ResetAllInClasses(root);
        }

        // ─── 내부 구현 ──────────────────────────────────────────────────

        private void SetupCards(VisualElement root, int showCount, IReadOnlyList<CardSlotManagerCompo.ChestUpgradeResult> results)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                var card = root.Q<VisualElement>("loot-card-" + i);
                if (card == null) continue;

                if (i >= showCount || i >= results.Count)
                {
                    card.style.display = DisplayStyle.None;
                    continue;
                }

                card.style.display = DisplayStyle.Flex;

                var data = results[i].Card;

                card.RemoveFromClassList("rarity--common");
                card.RemoveFromClassList("rarity--rare");
                card.RemoveFromClassList("rarity--legendary");
                card.AddToClassList(RarityToClass(data.rarity));

                var tier      = root.Q<Label>("loot-card-" + i + "-tier");
                var nameLabel = root.Q<Label>("loot-card-" + i + "-name");
                var desc      = root.Q<Label>("loot-card-" + i + "-desc");
                var icon      = root.Q<VisualElement>("loot-card-" + i + "-icon");

                if (tier      != null) tier.text      = RarityToTierLabel(data.rarity);
                if (nameLabel != null) nameLabel.text  = data.cardName;
                if (desc      != null) desc.text       = data.description;
                if (icon      != null && data.icon != null)
                    icon.style.backgroundImage = new StyleBackground(data.icon);
            }
        }

        private void PlayRevealAnimations(VisualElement root, int showCount)
        {
            int gen = ++_generation;

            var banner = root.Q<VisualElement>("loot-banner");
            if (banner != null)
                banner.schedule.Execute(() =>
                {
                    if (_generation != gen) return;
                    banner.AddToClassList("loot-banner--in");
                }).StartingIn(RevealStartMs);

            for (int i = 0; i < showCount; i++)
            {
                int idx  = i;
                var card = root.Q<VisualElement>("loot-card-" + idx);
                if (card == null) continue;

                long delay = RevealStartMs + idx * CardStaggerMs;
                card.schedule.Execute(() =>
                {
                    if (_generation != gen) return;
                    card.AddToClassList("loot-card--in");
                }).StartingIn(delay);
            }

            long completeMs = RevealStartMs
                + (showCount > 0 ? (long)(showCount - 1) * CardStaggerMs : 0L)
                + TransitionDurationMs
                + 100L;

            root.schedule.Execute(() =>
            {
                if (_generation != gen) return;
                _canClose = true;
                OnOpenCompleted?.Invoke();
            }).StartingIn(completeMs);
        }

        private static void ResetAllInClasses(VisualElement root)
        {
            root.Q<VisualElement>("loot-banner")?.RemoveFromClassList("loot-banner--in");
            for (int i = 0; i < MaxSlots; i++)
                root.Q<VisualElement>("loot-card-" + i)?.RemoveFromClassList("loot-card--in");
        }

        private static string RarityToClass(CardRarity rarity) => rarity switch
        {
            CardRarity.Rare      => "rarity--rare",
            CardRarity.Epic      => "rarity--legendary",
            CardRarity.Legendary => "rarity--legendary",
            _                    => "rarity--common",
        };

        private static string RarityToTierLabel(CardRarity rarity) => rarity switch
        {
            CardRarity.Rare      => "VIP",
            CardRarity.Epic      => "BACKSTAGE",
            CardRarity.Legendary => "BACKSTAGE",
            _                    => "GENERAL",
        };
    }
}
