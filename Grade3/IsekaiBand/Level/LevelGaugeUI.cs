using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.UI;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Code.LCH._02.Scripts.Level
{
    public class LevelGaugeUI : MonoBehaviour
    {
        [SerializeField] private RectTransform levelUpGauge;

        [Header("UI Toolkit")]
        [SerializeField] private UIDocument document;
        [SerializeField] private string fillElementName = "exp-fill";
        [SerializeField] private string textElementName = "exp-text";

        private VisualElement _toolkitGauge;
        private Label _toolkitText;

        private void OnEnable()
        {
            HideLegacyUI();
            BindToolkit();
            Bus<ExpChangeEvent>.OnEvent += UpdateGauge;
            Bus<LevelUpEvent>.OnEvent += OnLevelUp;
            SetScale(0f);
        }

        private void OnDisable()
        {
            Bus<ExpChangeEvent>.OnEvent -= UpdateGauge;
            Bus<LevelUpEvent>.OnEvent -= OnLevelUp;
        }

        private void UpdateGauge(ExpChangeEvent e)
        {
            SetScale(e.ExpRatio);
        }

        private void SetScale(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);

            if (_toolkitGauge == null)
                BindToolkit();

            if (_toolkitGauge != null)
                _toolkitGauge.style.width = Length.Percent(ratio * 100f);

            if (_toolkitText != null)
                _toolkitText.text = $"EXP {Mathf.RoundToInt(ratio * 100f)}%";
        }

        private void OnLevelUp(LevelUpEvent e)
        {
            SetScale(0f);
        }

        private void BindToolkit()
        {
            document ??= ToolkitUiRuntime.EnsureGameHudDocument(null);
            document ??= FindToolkitDocument();
            if (document == null || document.rootVisualElement == null) return;

            _toolkitGauge = document.rootVisualElement.Q<VisualElement>(fillElementName);
            _toolkitText = document.rootVisualElement.Q<Label>(textElementName);
        }

        private UIDocument FindToolkitDocument()
        {
            UIDocument local = GetComponentInParent<UIDocument>();
            if (HasElement(local))
                return local;

            foreach (UIDocument candidate in FindObjectsOfType<UIDocument>(true))
            {
                if (HasElement(candidate))
                    return candidate;
            }

            return null;
        }

        private bool HasElement(UIDocument candidate)
        {
            return candidate != null
                   && candidate.rootVisualElement != null
                   && candidate.rootVisualElement.Q<VisualElement>(fillElementName) != null;
        }

        private void HideLegacyUI()
        {
            if (levelUpGauge != null)
                levelUpGauge.gameObject.SetActive(false);
        }
    }
}
