using System.Collections.Generic;
using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using _Work.CHUH.Code.WeaponCombine;

namespace _Work.CHUH.Code.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class CraftingBenchToolkitController : MonoBehaviour
    {
        private static readonly WeaponType[] BagSlotOrder =
        {
            WeaponType.Bass, WeaponType.Guitar, WeaponType.Drum, WeaponType.Keyboard, WeaponType.Vocal
        };

        private const float DragGhostHalfSize = 60f;
        private const string VisibleClass = "craft-root--visible";
        private const int PanelSortingOrder = 150;

        [SerializeField] private CombineWeaponController combineController;

        [Header("Completed Instrument Icons")]
        [SerializeField] private Sprite bassIcon;
        [SerializeField] private Sprite guitarIcon;
        [SerializeField] private Sprite drumIcon;
        [SerializeField] private Sprite keyboardIcon;
        [SerializeField] private Sprite vocalIcon;

        private CraftingRecipeList _recipeView;
        private CraftingRecipeList RecipeView => _recipeView ??= new(GetCombineDisplayName, ApplyInstrumentIcon);
        private CraftingCombinedInventory _combinedInventory;
        private CraftingCombinedInventory CombinedInventory => _combinedInventory ??= new(GetCombineDisplayName);

        private UIDocument _document;
        private VisualElement _screen;
        private VisualElement _craftPanel;
        private VisualElement _craftSlotRow;
        private VisualElement _inventoryRow;
        private Button _recipeButton;
        private VisualElement _recipePopup;
        private ScrollView _recipeList;
        private Button _recipeCloseButton;
        private Button _combineButton;
        private VisualElement _dragGhost;
        private VisualElement _resultPopup;
        private Label _resultBadgeLabel;
        private VisualElement _resultIconBox;
        private VisualElement _resultIconImage;
        private Label _resultName;
        private Label _resultDesc;
        private Button _resultCloseButton;

        private readonly VisualElement[] _bagItems = new VisualElement[BagSlotOrder.Length];
        private readonly List<WeaponType> _placedWeapons = new();

        private PlayerAttackCompo _attackCompo;
        private WeaponBuildManager _buildManager;
        private PanelSettings _sourcePanelSettings;
        private PanelSettings _runtimePanelSettings;

        private WeaponType _draggingType = WeaponType.None;
        private bool _isVisible;
        private int _lastEscapeHandledFrame = -1;

        public bool IsVisible => _isVisible;

        private void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            EnsurePanelOrder();
            BindElements();
            EnsureComponents();
            ApplyVisibility();
            RefreshBag();
            Bus<WeaponUpgradeEvent>.OnEvent += HandleWeaponUpgrade;
        }

        private void OnDisable()
        {
            Bus<WeaponUpgradeEvent>.OnEvent -= HandleWeaponUpgrade;
            _isVisible = false;
            ApplyVisibility();
            SetPaused(false);
        }

        private void OnDestroy()
        {
            GameplayPauseService.Release(this);

            if (_document != null && _document.panelSettings == _runtimePanelSettings)
                _document.panelSettings = _sourcePanelSettings;

            if (_runtimePanelSettings != null)
                Destroy(_runtimePanelSettings);
        }

        private void Update()
        {
            if (GameplayUiBlockService.IsBlocked)
            {
                if (_isVisible)
                    Close();
                return;
            }

            if (Keyboard.current == null) return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame && TryHandleEscape())
                return;

            if (Keyboard.current.tabKey.wasPressedThisFrame)
                Toggle();
        }

        private void Toggle()
        {
            SetVisible(!_isVisible);
        }

        public void Open()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            SetVisible(true);
        }

        public void Close()
        {
            SetVisible(false);
        }

        public bool TryHandleEscape()
        {
            // Update 실행 순서와 관계없이 같은 Esc 입력으로 설정 창까지 열리지 않게 한다.
            if (_lastEscapeHandledFrame == Time.frameCount) return true;
            if (!_isVisible) return false;

            _lastEscapeHandledFrame = Time.frameCount;
            Close();
            return true;
        }

        public void RefreshInventory()
        {
            RefreshBag();
        }

        private void HandleWeaponUpgrade(WeaponUpgradeEvent evt)
        {
            if (_isVisible)
                RefreshBag();
        }

        private void SetVisible(bool isVisible)
        {
            if (_screen == null) return;
            if (isVisible && GameplayUiBlockService.IsBlocked) return;

            _isVisible = isVisible;
            ApplyVisibility();
            SetPaused(_isVisible);
            if (_isVisible)
                RefreshBag();
            else
            {
                HideRecipePopup();
                HideCombineResult();
            }
        }

        private void EnsurePanelOrder()
        {
            if (_document == null || _runtimePanelSettings != null) return;

            _sourcePanelSettings = _document.panelSettings;
            if (_sourcePanelSettings == null) return;

            _runtimePanelSettings = Instantiate(_sourcePanelSettings);
            _runtimePanelSettings.sortingOrder = PanelSortingOrder;
            _document.panelSettings = _runtimePanelSettings;
        }

        private void SetPaused(bool paused)
        {
            if (paused)
                GameplayPauseService.Acquire(this);
            else
                GameplayPauseService.Release(this);
        }

        private void ApplyVisibility()
        {
            if (_screen == null) return;

            // Slides up/down via the "top" transition on .craft-root (see CraftingBench.uss);
            // stays in the visual tree the whole time so the animation can play both ways.
            _screen.EnableInClassList(VisibleClass, _isVisible);
            _screen.pickingMode = _isVisible ? PickingMode.Position : PickingMode.Ignore;
        }

        private void BindElements()
        {
            VisualElement root = _document != null ? _document.rootVisualElement : null;
            if (root == null) return;

            _screen = root.Q<VisualElement>("craft-root");
            _craftPanel = root.Q<VisualElement>("craft-panel");
            _craftSlotRow = root.Q<VisualElement>("craft-slot-row");
            _inventoryRow = root.Q<VisualElement>("inventory-row");
            _dragGhost = root.Q<VisualElement>("drag-ghost");

            _recipeButton = root.Q<Button>("recipe-button");
            if (_recipeButton != null)
            {
                _recipeButton.clicked -= ShowRecipePopup;
                _recipeButton.clicked += ShowRecipePopup;
            }

            _recipePopup = root.Q<VisualElement>("recipe-popup");
            _recipeList = root.Q<ScrollView>("recipe-list");
            _recipeCloseButton = root.Q<Button>("recipe-close-button");
            if (_recipeCloseButton != null)
            {
                _recipeCloseButton.clicked -= HideRecipePopup;
                _recipeCloseButton.clicked += HideRecipePopup;
            }

            _combineButton = root.Q<Button>("combine-button");
            if (_combineButton != null)
            {
                _combineButton.clicked -= HandleCombineClicked;
                _combineButton.clicked += HandleCombineClicked;
            }

            _resultPopup = root.Q<VisualElement>("result-popup");
            _resultBadgeLabel = root.Q<Label>(className: "result-badge-text");
            _resultIconBox = root.Q<VisualElement>("result-icon");
            _resultIconImage = root.Q<VisualElement>("result-icon-image");
            _resultName = root.Q<Label>("result-name");
            _resultDesc = root.Q<Label>("result-desc");
            _resultCloseButton = root.Q<Button>("result-close-button");
            if (_resultCloseButton != null)
            {
                _resultCloseButton.clicked -= HideCombineResult;
                _resultCloseButton.clicked += HideCombineResult;
            }

            for (int i = 0; i < BagSlotOrder.Length; i++)
                BindBagItem(root, i);
        }

        private void BindBagItem(VisualElement root, int index)
        {
            VisualElement item = root.Q<VisualElement>($"item-slot-{index}");
            if (_bagItems[index] == item) return;

            if (_bagItems[index] != null)
            {
                _bagItems[index].UnregisterCallback<PointerDownEvent>(OnBagItemPointerDown);
                _bagItems[index].UnregisterCallback<PointerMoveEvent>(OnBagItemPointerMove);
                _bagItems[index].UnregisterCallback<PointerUpEvent>(OnBagItemPointerUp);
            }

            _bagItems[index] = item;
            if (item == null) return;

            item.userData = index;
            item.pickingMode = PickingMode.Position;
            item.RegisterCallback<PointerDownEvent>(OnBagItemPointerDown);
            item.RegisterCallback<PointerMoveEvent>(OnBagItemPointerMove);
            item.RegisterCallback<PointerUpEvent>(OnBagItemPointerUp);
        }

        private void EnsureComponents()
        {
            if (combineController == null)
                combineController = FindBestCombineController();
            if (combineController == null) return;

            if (_attackCompo == null)
                _attackCompo = combineController.GetComponent<PlayerAttackCompo>();
            if (_buildManager == null && _attackCompo != null)
                _buildManager = WeaponBuildManager.GetOrCreate(_attackCompo);
        }

        private static CombineWeaponController FindBestCombineController()
        {
#if UNITY_2023_1_OR_NEWER
            var all = FindObjectsByType<CombineWeaponController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var all = FindObjectsOfType<CombineWeaponController>(true);
#endif
            return all != null && all.Length > 0 ? all[0] : null;
        }

        private void RefreshBag()
        {
            EnsureComponents();

            for (int i = 0; i < BagSlotOrder.Length; i++)
            {
                WeaponType type = BagSlotOrder[i];
                VisualElement item = _bagItems[i];
                if (item == null) continue;

                bool owned = _attackCompo != null && _attackCompo.HasWeapon(type);
                item.style.display = owned ? DisplayStyle.Flex : DisplayStyle.None;
                if (!owned) continue;

                VisualElement icon = item.Q<VisualElement>(className: "inventory-icon");
                ApplyInstrumentIcon(icon, type);

                Label label = item.Q<Label>(className: "inventory-label");
                if (label != null)
                    label.text = InstrumentPartRules.GetInstrumentDisplayName(type);

                bool placed = _placedWeapons.Contains(type);
                item.EnableInClassList("inventory-item--placed", placed);

                bool full = _buildManager != null && _buildManager.IsInstrumentComplete(type);
                item.EnableInClassList("inventory-item--locked", !full && !placed);
                RefreshInstrumentProgress(item, type);
            }

            CombinedInventory.Refresh(_inventoryRow, combineController);
        }

        private void RefreshInstrumentProgress(VisualElement item, WeaponType type)
        {
            VisualElement fill = item.Q<VisualElement>(className: "inventory-progress-fill");
            if (fill == null) return;

            int count = _buildManager != null ? _buildManager.GetCollectedPartCount(type) : 0;
            float progress = Mathf.Clamp01((float)count / InstrumentPartRules.RequiredPartCount);
            fill.style.height = Length.Percent(progress * 100f);
        }

        private bool IsDraggable(WeaponType type)
        {
            return _attackCompo != null && _attackCompo.HasWeapon(type)
                   && _buildManager != null && _buildManager.IsInstrumentComplete(type)
                   && !_placedWeapons.Contains(type);
        }

        private void OnBagItemPointerDown(PointerDownEvent evt)
        {
            if (evt.currentTarget is not VisualElement item || item.userData is not int index) return;

            WeaponType type = BagSlotOrder[index];
            if (!IsDraggable(type)) return;

            _draggingType = type;
            item.CapturePointer(evt.pointerId);
            ShowDragGhost(type, evt.position);
            evt.StopPropagation();
        }

        private void OnBagItemPointerMove(PointerMoveEvent evt)
        {
            if (_draggingType == WeaponType.None) return;
            MoveDragGhost(evt.position);
        }

        private void OnBagItemPointerUp(PointerUpEvent evt)
        {
            if (_draggingType == WeaponType.None) return;
            if (evt.currentTarget is VisualElement item)
                item.ReleasePointer(evt.pointerId);

            HideDragGhost();

            if (IsOverCraftPanel(evt.position))
                PlaceInCraftSlot(_draggingType);

            _draggingType = WeaponType.None;
        }

        private bool IsOverCraftPanel(Vector2 position)
        {
            return _craftPanel != null && _craftPanel.worldBound.Contains(position);
        }

        private void ShowDragGhost(WeaponType type, Vector2 position)
        {
            if (_dragGhost == null) return;
            ApplyInstrumentIcon(_dragGhost, type);
            _dragGhost.style.display = DisplayStyle.Flex;
            MoveDragGhost(position);
        }

        private void MoveDragGhost(Vector2 position)
        {
            if (_dragGhost == null) return;
            _dragGhost.style.left = position.x - DragGhostHalfSize;
            _dragGhost.style.top = position.y - DragGhostHalfSize;
        }

        private void HideDragGhost()
        {
            if (_dragGhost != null)
                _dragGhost.style.display = DisplayStyle.None;
        }

        private void PlaceInCraftSlot(WeaponType type)
        {
            if (_craftSlotRow == null || _placedWeapons.Contains(type)) return;

            _placedWeapons.Add(type);
            AddCraftSlotElement(type);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.CraftingInstrumentPlaced,
                SoundType.SFX));
            RefreshBag();
            UpdateCombineButtonState();
        }

        private void AddCraftSlotElement(WeaponType type)
        {
            var slot = new VisualElement { userData = type, pickingMode = PickingMode.Position };
            slot.AddToClassList("craft-slot");
            slot.AddToClassList("craft-slot--filled");

            var icon = new VisualElement();
            icon.AddToClassList("craft-slot-icon");
            ApplyInstrumentIcon(icon, type);
            slot.Add(icon);

            var label = new Label(InstrumentPartRules.GetInstrumentDisplayName(type));
            label.AddToClassList("craft-slot-label");
            slot.Add(label);

            slot.RegisterCallback<ClickEvent>(OnCraftSlotClicked);
            _craftSlotRow.Add(slot);
        }

        private void ApplyInstrumentIcon(VisualElement element, WeaponType type)
        {
            if (element == null) return;

            Sprite icon = GetInstrumentIcon(type);
            if (icon != null)
            {
                element.style.backgroundImage = new StyleBackground(icon);
                element.style.backgroundColor = Color.clear;
                return;
            }

            element.style.backgroundImage = StyleKeyword.None;
            element.style.backgroundColor = InstrumentPartRules.GetInstrumentColor(type);
        }

        private Sprite GetInstrumentIcon(WeaponType type) => type switch
        {
            WeaponType.Bass => bassIcon,
            WeaponType.Guitar => guitarIcon,
            WeaponType.Drum => drumIcon,
            WeaponType.Keyboard => keyboardIcon,
            WeaponType.Vocal => vocalIcon,
            _ => null
        };

        private void OnCraftSlotClicked(ClickEvent evt)
        {
            if (evt.currentTarget is not VisualElement slot || slot.userData is not WeaponType type) return;
            RemoveFromCraftSlot(type, slot);
        }

        private void RemoveFromCraftSlot(WeaponType type, VisualElement slotElement)
        {
            _placedWeapons.Remove(type);
            slotElement.RemoveFromHierarchy();
            RefreshBag();
            UpdateCombineButtonState();
        }

        private void ClearCraftSlots()
        {
            _placedWeapons.Clear();
            _craftSlotRow?.Clear();
            UpdateCombineButtonState();
        }

        private void UpdateCombineButtonState()
        {
            if (_combineButton == null) return;
            _combineButton.EnableInClassList("combine-button--enabled", _placedWeapons.Count >= 2);
        }

        private void ShowRecipePopup()
        {
            if (_recipePopup == null || _recipeList == null) return;

            EnsureComponents();
            RecipeView.Populate(_recipeList, () => combineController?.GetAllCombinations());
            HideCombineResult();
            _recipePopup.style.display = DisplayStyle.Flex;
        }

        private void HideRecipePopup()
        {
            if (_recipePopup != null)
                _recipePopup.style.display = DisplayStyle.None;
        }

        private void HandleCombineClicked()
        {
            if (_placedWeapons.Count < 2 || combineController == null) return;

            CombineMakeDataSO matched = FindMatchingRecipe();
            if (matched == null || !combineController.TryCombine(matched))
            {
                ShowCombineFailure();
                return;
            }

            // TryCombine already removed the ingredient weapons and granted the
            // combined weapon (see CombineWeaponController.TryCombine).
            ClearCraftSlots();
            RefreshBag();
            ShowCombineResult(matched);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.CraftingSucceeded,
                SoundType.SFX));
        }

        private CombineMakeDataSO FindMatchingRecipe()
        {
            foreach (CombineMakeDataSO recipe in combineController.GetAvailableCombinations())
            {
                if (recipe.needWeapons == null || recipe.needWeapons.Count != _placedWeapons.Count) continue;

                bool matches = true;
                foreach (WeaponType needed in recipe.needWeapons)
                {
                    if (_placedWeapons.Contains(needed)) continue;
                    matches = false;
                    break;
                }

                if (matches) return recipe;
            }

            return null;
        }

        private const string SuccessBadgeText = "조합 완료!";
        private const string FailureBadgeText = "조합 실패";
        private const string FailureDescText = "이 조합은 어울리지 않는 듯 하다...";

        private void ShowCombineResult(CombineMakeDataSO recipe)
        {
            if (_resultPopup == null) return;

            HideRecipePopup();

            if (_resultBadgeLabel != null)
                _resultBadgeLabel.text = SuccessBadgeText;
            if (_resultIconBox != null)
                _resultIconBox.style.display = DisplayStyle.Flex;
            if (_resultIconImage != null)
                _resultIconImage.style.backgroundImage = recipe.icon != null
                    ? new StyleBackground(recipe.icon)
                    : new StyleBackground(StyleKeyword.None);
            if (_resultName != null)
            {
                _resultName.style.display = DisplayStyle.Flex;
                _resultName.text = GetCombineDisplayName(recipe.combineWeapon);
            }
            if (_resultDesc != null)
                _resultDesc.text = recipe.description;

            _resultPopup.style.display = DisplayStyle.Flex;
        }

        // Shown when the placed instruments don't match any recipe.
        private void ShowCombineFailure()
        {
            if (_resultPopup == null) return;

            if (_resultBadgeLabel != null)
                _resultBadgeLabel.text = FailureBadgeText;
            if (_resultIconBox != null)
                _resultIconBox.style.display = DisplayStyle.None;
            if (_resultName != null)
                _resultName.style.display = DisplayStyle.None;
            if (_resultDesc != null)
                _resultDesc.text = FailureDescText;

            _resultPopup.style.display = DisplayStyle.Flex;
        }

        private void HideCombineResult()
        {
            if (_resultPopup != null)
                _resultPopup.style.display = DisplayStyle.None;
        }

        private static string GetCombineDisplayName(CombineWeaponType type) => type switch
        {
            CombineWeaponType.EmotionalDuo => "감성 듀오",
            CombineWeaponType.RhythmSection => "리듬 섹션",
            CombineWeaponType.JazzDuo => "재즈 듀오",
            CombineWeaponType.RockStarDuo => "락스타 듀오",
            CombineWeaponType.OrthodoxRockBand => "정통 록 밴드",
            CombineWeaponType.HardRockBand => "하드 록 밴드",
            CombineWeaponType.JazzBand => "재즈 밴드",
            CombineWeaponType.PopRockBand => "팝 록 밴드",
            CombineWeaponType.RockBand => "락 밴드",
            CombineWeaponType.BalladBand => "발라드 밴드",
            CombineWeaponType.PunkBand => "펑크 밴드",
            CombineWeaponType.SymphonicRock => "심포닉 록",
            CombineWeaponType.JazzPopBand => "재즈 팝 밴드",
            CombineWeaponType.FusionJazzBand => "퓨전 재즈 밴드",
            CombineWeaponType.EmotionalRockBand => "감성 록 밴드",
            CombineWeaponType.FullBand => "완전체 밴드",
            _ => type.ToString()
        };
    }
}
