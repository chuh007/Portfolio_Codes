using UnityEngine;
using UnityEngine.UI;
using Work.Chipmunk._01.Scripts.Combat;

namespace Work.CHUH._01Scripts.Combat
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        
        private Image _hpBar;
        
        private void Awake()
        {
            _hpBar = GetComponent<Image>();
            health.OnHitEvent.AddListener(HandleHit);
        }

        private void HandleHit(float value)
        {
            _hpBar.fillAmount = value;
        }
    }
}