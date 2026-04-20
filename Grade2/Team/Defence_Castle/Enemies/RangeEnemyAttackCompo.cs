using System;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CHUH._01Scripts.Combat;
using Work.CHUH._01Scripts.Entities;

namespace Work.CHUH._01Scripts.Enemies
{
    public class RangeEnemyAttackCompo : EntityAttackCompo
    {
        [Header("Pool")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private Projectile arrowPrefab;
        [SerializeField] private PoolItemSO arrowSO;
        
        [SerializeField] private Transform muzzle;
        
        
        protected override void Attack()
        {
            base.Attack();
            EnemyArrow arrow = poolManager.Pop(arrowSO) as EnemyArrow;
            arrow.transform.position = muzzle.transform.position;
            arrow.InitAndFire(Vector2.left, _damage, 10f, _owner, whatIsTarget);
        }
        
    }
}