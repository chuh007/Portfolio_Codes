using _01Scripts.Entities;
using _01Scripts.Players;
using UnityEngine;

namespace _01Scripts.Combat
{
    public interface IDamageable
    {
        public void ApplyDamage(DamageData damageData, Entity dealer);
    }
}