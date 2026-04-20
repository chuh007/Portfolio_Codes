using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Work.CHUH._01Scripts.Combat;

namespace Work.CHUH._01Scripts.UI
{
    public class EntityHPBar : MonoBehaviour
    {
        [SerializeField] private GameObject hpBarObject;
        [SerializeField] private EntityHealth entityHealth;
        [SerializeField] private TextMeshProUGUI hpText;
        
        private Image _hpBar;

        private void Awake()
        {
            _hpBar = GetComponent<Image>();
            entityHealth.OnHPChange += HandleHPChange;
        }

        private void HandleHPChange(float value, float maxValue)
        {
            if(value <= 0.01f) hpBarObject?.SetActive(false);
            else if(Mathf.Approximately(value, maxValue)) hpBarObject?.SetActive(true);
            _hpBar.fillAmount = value / maxValue;
            hpText?.SetText(value.ToString());
        }
    }
}