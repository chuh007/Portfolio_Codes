using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class CraftingBenchToolkitController : MonoBehaviour
    {
        [SerializeField] private CombineWeaponController combineController;
        [Header("Completed Instrument Icons")]
        [SerializeField] private Sprite bassIcon;
        [SerializeField] private Sprite guitarIcon;
        [SerializeField] private Sprite drumIcon;
        [SerializeField] private Sprite keyboardIcon;
        [SerializeField] private Sprite vocalIcon;

        private UIDocument _document;
        private PanelSettings _sourcePanelSettings;
        private PanelSettings _runtimePanelSettings;
        private CraftingBuildContext _context;
        private CraftingBenchView _view;
        private bool _isVisible;
        private bool _refreshPending;
        private int _lastEscapeHandledFrame = -1;
        public bool IsVisible => _isVisible;

        private void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            EnsurePanelOrder();
            _context = new CraftingBuildContext(GetInstrumentIcon);
            VisualElement root = _document.rootVisualElement.Q("craft-root");
            if (root != null) _view = new CraftingBenchView(root, _context, Close);
            _view?.SetVisible(false);
            Bus<WeaponUpgradeEvent>.OnEvent += HandleWeaponUpgrade;
            Bus<CardEquippedEvent>.OnEvent += HandleCardEquipped;
            Bus<WeaponSlotsChangedEvent>.OnEvent += HandleSlotsChanged;
        }

        private void OnDisable()
        {
            Bus<WeaponUpgradeEvent>.OnEvent -= HandleWeaponUpgrade;
            Bus<CardEquippedEvent>.OnEvent -= HandleCardEquipped;
            Bus<WeaponSlotsChangedEvent>.OnEvent -= HandleSlotsChanged;
            Close();
            _view?.Dispose();
            _view = null;
        }

        private void OnDestroy()
        {
            GameplayPauseService.Release(this);
            if (_document != null && _document.panelSettings == _runtimePanelSettings)
                _document.panelSettings = _sourcePanelSettings;
            if (_runtimePanelSettings != null) Destroy(_runtimePanelSettings);
        }

        private void Update()
        {
            if (GameplayUiBlockService.IsBlocked)
            {
                if (_isVisible) Close();
                return;
            }
            if (Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame && TryHandleEscape()) return;
                if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    if (_isVisible) Close();
                    else Open();
                }
            }
            if (_isVisible && _refreshPending) RefreshInventory();
        }

        public void Open()
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (_view == null || GameplayUiBlockService.IsBlocked) return;
            BindPlayer();
            _isVisible = true;
            _view.SetVisible(true);
            GameplayPauseService.Acquire(this);
            _view.Refresh(true);
            _refreshPending = false;
        }

        public void Close()
        {
            _isVisible = false;
            _refreshPending = false;
            _view?.SetVisible(false);
            GameplayPauseService.Release(this);
        }

        public bool TryHandleEscape()
        {
            // 하나의 Esc 입력이 Tab과 설정 화면에서 동시에 처리되지 않도록 한다.
            if (_lastEscapeHandledFrame == Time.frameCount) return true;
            if (!_isVisible) return false;
            _lastEscapeHandledFrame = Time.frameCount;
            if (!_view.TryClosePopup()) Close();
            return true;
        }

        public void RefreshInventory()
        {
            if (_context == null || _view == null) return;
            BindPlayer();
            _view.Refresh(false);
            _refreshPending = false;
        }

        private void BindPlayer()
        {
            if (combineController == null)
                combineController = FindFirstObjectByType<CombineWeaponController>();
            _context.Bind(combineController);
        }

        private void HandleWeaponUpgrade(WeaponUpgradeEvent evt) => _refreshPending = true;
        private void HandleCardEquipped(CardEquippedEvent evt) => _refreshPending = true;
        private void HandleSlotsChanged(WeaponSlotsChangedEvent evt) => _refreshPending = true;

        private void EnsurePanelOrder()
        {
            if (_runtimePanelSettings != null || _document.panelSettings == null) return;
            _sourcePanelSettings = _document.panelSettings;
            _runtimePanelSettings = Instantiate(_sourcePanelSettings);
            _runtimePanelSettings.sortingOrder = 150;
            _document.panelSettings = _runtimePanelSettings;
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
    }
}
