using System.Collections.Generic;
using _01Scripts.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _01Scripts.Combat
{
    public struct DamageData
    {
        public float damage;
    }

    public abstract class EntityAttackCompo : MonoBehaviour, IEntityComponent
    {
        public AttackDataSO currentAttackData;

        [SerializeField] protected StatSO damageStat;
        public List<AttackDataSO> attackDataList;

        protected Entity _entity, _target;
        protected EntityAnimator _entityAnimator;
        protected EntityStat _entityStat;

        protected float _baseDamage;
        protected float _damage;
        
        private Dictionary<string, AttackDataSO> _realData;
        
        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
            _realData = new Dictionary<string, AttackDataSO>();
            foreach (var attackDataSO in attackDataList)
            {
                _realData.Add(attackDataSO.attackName, attackDataSO);
            }
            _entityAnimator = _entity.GetCompo<EntityAnimator>();
            _entityStat = _entity.GetCompo<EntityStat>();
            _baseDamage = _entityStat.GetStat(damageStat).Value;
        }
        
        public virtual void Attack()
        {
            _baseDamage = _entityStat.GetStat(damageStat).Value;
            _damage = _baseDamage * currentAttackData.damageMultiplier +
                      currentAttackData.damageIncrease;
        }

        public virtual void EndAttack()
        {
        }

        public AttackDataSO GetAttackData(string attackName)
        {
            return _realData.GetValueOrDefault(attackName);
        }
    }
}