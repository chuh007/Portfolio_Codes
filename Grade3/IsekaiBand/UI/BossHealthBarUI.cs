using System.Threading;
using _Work.CHUH.Code.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.UI
{
    public class BossHealthBarUI : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private GameObject healthCompo;
        [SerializeField] private string bossName = "강한 보스";
        [SerializeField] private bool hideLegacyChildren = true;

        private IHealth _health;
        private readonly BossHealthBarDocument _document = new();
        private readonly BossHealthBarDisplay _display = new();
        private BossHealthBarEntrance _entrance;
        private BossHealthBarEntrance Entrance => _entrance ??= new(this, _display, () => _health, CacheElements);

        private void Awake()
        {
            // 비활성 상태로 배치된 적은 문서를 생성하지 않는다.
            if (!enabled)
                return;

            if (hideLegacyChildren)
                HideLegacyChildren();

            if (visualTreeAsset == null)
            {
                Debug.LogError($"{nameof(BossHealthBarUI)} needs a VisualTreeAsset.", this);
                enabled = false;
                return;
            }

            if (healthCompo == null || !healthCompo.TryGetComponent(out _health))
            {
                Debug.LogError($"{nameof(BossHealthBarUI)} needs an IHealth component.", this);
                enabled = false;
                return;
            }

            _document.CreateDocument(transform, panelSettingsAsset, visualTreeAsset);
        }

        private void Start()
        {
            if (_document.Document == null || _health == null)
                return;

            if (!CacheElements())
                return;

            _health.OnHpChanged += HandleHpChanged;
            if (Entrance.IsHidden)
                _display.SetDisplayedRatio(0f, false);
            else
                HandleHpChanged(_health.CurrentHealth);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnHpChanged -= HandleHpChanged;

            _document.Dispose();
        }

        private void HideLegacyChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        private void HandleHpChanged(float currentHealth)
        {
            if (Entrance.IsPlaying || Entrance.IsHidden)
                return;

            if (_health == null || !_display.IsReady)
                return;

            float ratio = _health.MaxHealth <= 0f ? 0f : Mathf.Clamp01(currentHealth / _health.MaxHealth);
            _display.SetDisplayedRatio(ratio, true);
        }

        private bool CacheElements() => _display.CacheElements(_document.Document, bossName, !Entrance.IsHidden);

        public void HideUntilFillAnimation() => Entrance.HideUntilFillAnimation();

        public UniTask<bool> PlayFillAnimationAsync(float duration, CancellationToken cancellationToken)
            => Entrance.PlayFillAnimationAsync(duration, cancellationToken);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (panelSettingsAsset == null)
                Debug.LogError("Please enter PanelSettings.", this);

            if (healthCompo == null)
                return;

            if (!healthCompo.TryGetComponent<IHealth>(out _))
            {
                healthCompo = null;
                Debug.LogError("Please enter an IHealth component.", this);
            }
        }
#endif
    }
}
