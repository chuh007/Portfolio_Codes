using Code.Combats;
using Code.Core.EventSystems;
using Code.Core.StatSystem;
using Code.Entities;
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Code.Enemies
{
    public class EnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInit
    {
        [Header("Atk settings")] 
        public float attackDistance;
        public float detectDistance;
        
        [SerializeField] private float attackCooldown, cooldownRandomness;
        [SerializeField] private StatSO damageStat;
        
        [Header("Reference")]
        [SerializeField] private DamageCaster damageCaster;
        [SerializeField] private string attackRangeName, detectRangeName, attackCooldownName;

        private BTEnemy _enemy;
        private BlackboardVariable<float> _attackCooldownVariable;
        private EntityAnimationTrigger _triggerCompo;
        private EntityStat _statCompo;
        private float damage = 5f;

        public void Initialize(Entity entity)
        {
            _enemy = entity as BTEnemy;
            _triggerCompo = entity.GetCompo<EntityAnimationTrigger>();
            _statCompo = entity.GetCompo<EntityStat>();
            damageCaster.InitCaster(entity);
            Debug.Assert(_enemy != null, $"Not corrected entity - enemy attack component [{entity.gameObject.name}]");
            _enemy.GetBlackboardVariable<float>(attackRangeName).Value = attackDistance;
            _enemy.GetBlackboardVariable<float>(detectRangeName).Value = detectDistance;
            _attackCooldownVariable = _enemy.GetBlackboardVariable<float>(attackCooldownName);
            damage = _statCompo.GetStat(damageStat).Value;
        }

        public void AfterInit()
        {
            _triggerCompo.OnAttackTrigger += HandleAttackTrigger;
        }

        private void OnDestroy()
        {
            _triggerCompo.OnAttackTrigger -= HandleAttackTrigger;
        }

        private void HandleAttackTrigger()
        {

            Vector2 knockBackForce = new Vector2(1f, 1f);
            bool success = damageCaster.CastDamage(damage, knockBackForce, false);

        }
    }
}