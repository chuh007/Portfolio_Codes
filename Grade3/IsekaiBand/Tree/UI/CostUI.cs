using System;
using _Work.CHUH.Code.Resource;
using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.Tree.UI
{
    public class CostUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costText;

        private SupplyManager _supplyManager;

        private void Start()
        {
            BindSupplyManager(true);
        }

        private void OnEnable()
        {
            BindSupplyManager();
        }

        private void OnDisable()
        {
            UnbindSupplyManager();
        }

        private void BindSupplyManager(bool logIfMissing = false)
        {
            SupplyManager currentManager = SupplyManager.Instance;
            if (currentManager == null)
            {
                if (logIfMissing)
                    Debug.LogWarning("[CostUI] SupplyManager is missing in scene.", this);

                return;
            }

            if (_supplyManager != currentManager)
            {
                UnbindSupplyManager();
                _supplyManager = currentManager;
                _supplyManager.OnCoinChanged += HandleCoinChange;
            }

            RefreshCoin();
        }

        private void UnbindSupplyManager()
        {
            if (_supplyManager == null)
                return;

            _supplyManager.OnCoinChanged -= HandleCoinChange;
            _supplyManager = null;
        }

        private void RefreshCoin()
        {
            costText.SetText(_supplyManager.Coin.ToString());
        }

        private void HandleCoinChange(int coin)
        {
            costText.SetText(coin.ToString());
        }
    }
}
