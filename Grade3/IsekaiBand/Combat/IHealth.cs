using System;

using _Code.LCH._02.Scripts.Combat;

namespace _Work.CHUH.Code.Combat
{
    public interface IMinimumHealthPolicy
    {
        float MinimumHealth { get; }
    }

    public interface IHealthDepletionHandler
    {
        bool TryHandleHealthDepleted();
    }

    public interface IHealth
    {
        public float MaxHealth { get; }
        public float CurrentHealth { get; }
        public Action<float> OnHpChanged { get; set; }
        
        public void TakeDamage(float damage, bool ignoreDefense = false, DamageType damageType = DamageType.Physical, bool isFixedDamage = false);
        public void TakeHeal(float heal);
    }
}
