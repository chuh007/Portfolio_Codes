using Code.Core.EventSystems;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class HPBarValueSetter : MonoBehaviour
    {
        [SerializeField] private Image HPBar;
        [SerializeField] private TextMeshProUGUI HPText;
        [SerializeField] private GameEventChannelSO uiChannel;

        private void Awake()
        {
            uiChannel.AddListener<HPValueChangeEvent>(UIValueSet);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<HPValueChangeEvent>(UIValueSet);
        }

        private void UIValueSet(HPValueChangeEvent evt)
        {
            HPBar.fillAmount = evt.value / evt.maxValue;
            HPText.text = $"{evt.value} / {evt.maxValue}";
        }
    }
}

