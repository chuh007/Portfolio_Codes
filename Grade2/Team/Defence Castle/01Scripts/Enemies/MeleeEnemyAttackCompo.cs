using System;
using UnityEngine;
using Work.CHUH._01Scripts.Combat;
using Work.CHUH._01Scripts.Entities;

namespace Work.CHUH._01Scripts.Enemies
{
    public class MeleeEnemyAttackCompo : EntityAttackCompo
    {
        protected override void Attack()
        {
            base.Attack();
            _target?.TakeDamage(_damage);
        }
    }
}