using _01Scripts.Enemies;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Combat
{
    public class EnemyAttackCompo : EntityAttackCompo
    {
        private Enemy _owner;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            _owner = entity as Enemy;
            _target = _owner.target;
        }
        
        public override void Attack()
        {
            base.Attack();
            DamageData data = new DamageData();
            data.damage = _damage;
            if(_target.TryCastDamage())
                _target.ApplyDamage(data, _entity);
        }

        public int GetRandomAttack()
        {
            currentAttackData = GetAttackData(attackDataList[Random.Range(0, attackDataList.Count)].attackName);
            return Animator.StringToHash(currentAttackData.attackAnimationName);
        }
    }
}