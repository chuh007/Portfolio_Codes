using System;
using _Work.CHUH.Code.Resource;
using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.Tree.UI
{
    public class TreeResourcesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI noteText;

        private void Awake()
        {
            SupplyManager.Instance.OnCoinChanged += HandleCoinChanged;
            SupplyManager.Instance.OnNoteChanged += HandleNoteChanged;
        }

        private void OnEnable()
        {
            goldText.SetText($"{SupplyManager.Instance.Coin}"); 
            noteText.SetText($"{SupplyManager.Instance.Note}");
        }

        private void HandleNoteChanged(int obj)
        {
            noteText.SetText($"{obj}");
        }

        private void HandleCoinChanged(int obj)
        {
            goldText.SetText($"{obj}");
        }
        

        private void OnDestroy()
        {
            if (SupplyManager.Instance == null) return;
            SupplyManager.Instance.OnCoinChanged -= HandleCoinChanged;
            SupplyManager.Instance.OnNoteChanged -= HandleNoteChanged;
        }
    }
}
