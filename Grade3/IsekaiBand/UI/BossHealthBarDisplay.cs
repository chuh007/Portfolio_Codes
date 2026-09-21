using System.Threading;
using _Work.CHUH.Code.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.UI
{
    internal class BossHealthBarDisplay
    {
        private VisualElement _fill, _trail, _root;
        private Label _nameLabel, _valueLabel;
        public bool IsReady => _fill != null && _trail != null;

        public bool CacheElements(UIDocument document, string bossName, bool visible)
        {
            if (document == null || document.rootVisualElement == null)
                return false;

            VisualElement root = document.rootVisualElement;
            _root = root.Q<VisualElement>("BossHpRoot");
            _fill = root.Q<VisualElement>("BossHpFill");
            _trail = root.Q<VisualElement>("BossHpTrail");
            _nameLabel = root.Q<Label>("BossName");
            _valueLabel = root.Q<Label>("BossHpValue");

            if (_nameLabel != null)
                _nameLabel.text = bossName;

            SetRootVisible(visible);

            return _fill != null && _trail != null;
        }

        public void SetRootVisible(bool visible)
        {
            if (_root != null)
                _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetDisplayedRatio(float ratio, bool updateCriticalState)
        {
            ratio = Mathf.Clamp01(ratio);
            Length width = Length.Percent(ratio * 100f);
            _fill.style.width = width;
            _trail.style.width = width;

            if (_valueLabel != null)
                _valueLabel.text = $"{Mathf.CeilToInt(ratio * 100f)}%";

            if (updateCriticalState)
                _root?.EnableInClassList("boss-hp--critical", ratio <= 0.25f);
        }

    }
}
