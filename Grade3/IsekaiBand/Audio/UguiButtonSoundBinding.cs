using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UguiButton = UnityEngine.UI.Button;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class UguiButtonSoundBinding
    {
        private readonly HashSet<UguiButton> _boundButtons = new();
        private readonly HashSet<UguiButton> _excludedButtons = new();
        private readonly UnityAction _playClick;

        public UguiButtonSoundBinding(UnityAction playClick) => _playClick = playClick;

        public void Exclude(UguiButton button)
        {
            _excludedButtons.Add(button);
            if (_boundButtons.Remove(button))
                button.onClick.RemoveListener(_playClick);
        }

        public void Refresh()
        {
            _boundButtons.RemoveWhere(button => button == null);
            _excludedButtons.RemoveWhere(button => button == null);
            UguiButton[] buttons = Object.FindObjectsByType<UguiButton>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (UguiButton button in buttons)
            {
                if (_excludedButtons.Contains(button)) continue;
                if (_boundButtons.Add(button))
                    button.onClick.AddListener(_playClick);
            }
        }

        public void Clear()
        {
            foreach (UguiButton button in _boundButtons)
            {
                if (button != null)
                    button.onClick.RemoveListener(_playClick);
            }
            _boundButtons.Clear();
        }
    }
}
