using System;
using _01Scripts.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private EntityHealthComponent healthComponent;
        
        public void ChangeHp(float value)
        {
            healthBar.fillAmount = value / healthComponent.maxHealth;
        }
    }
}