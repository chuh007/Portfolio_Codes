using System;
using _Work.CHUH.Code.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Temp
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private Image delayedHealthBar;
        [SerializeField] private GameObject healthCompo;
        [SerializeField] private float delayBarSpeed = 0.5f;
        
        private IHealth _health;
        private float _targetFillAmount = 1f;
        
        private void Awake()
        {
            _health = healthCompo.GetComponent<IHealth>();
            _health.OnHpChanged += HandleHpChanged;
        }

        private void Update()
        {
            if (delayedHealthBar == null) return;
            if (delayedHealthBar.fillAmount <= _targetFillAmount) return;

            delayedHealthBar.fillAmount = Mathf.MoveTowards(
                delayedHealthBar.fillAmount,
                _targetFillAmount,
                delayBarSpeed * Time.deltaTime);
        }

        private void HandleHpChanged(float value)
        {
            _targetFillAmount = value / _health.MaxHealth;
            healthBar.fillAmount = _targetFillAmount;

            if (delayedHealthBar != null && delayedHealthBar.fillAmount < _targetFillAmount)
            {
                delayedHealthBar.fillAmount = _targetFillAmount;
            }
        }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (healthCompo != null)
            {
                if (!healthCompo.TryGetComponent<IHealth>(out IHealth health))
                {
                    healthCompo = null;
                    Debug.LogError("Please Enter IHealth Component");
                }
            }
        }
        
        #endif
    }
}
