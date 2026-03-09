using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Combat
{

    public class PlayerAttackCompo : EntityAttackCompo
    {
        private int qteSuccesCnt = 0;
        
        public void SetTarget(Entity target)
        {
            _target = target;
        }

        public void QteSuccess() => qteSuccesCnt++;
        
        public override void Attack()
        {
            base.Attack();
            DamageData data = new DamageData();
            float baseDamage = _damage;
            data.damage = baseDamage;
            if (qteSuccesCnt == 0)
                data.damage = baseDamage * 0.5f;
            else if (qteSuccesCnt == currentAttackData.triggerCount)
                data.damage = baseDamage * 1.5f;
            else
            {
                int success = qteSuccesCnt;
                int fail = currentAttackData.triggerCount - qteSuccesCnt;
                data.damage = baseDamage * (1.0f + (success - fail) * 0.1f);
            }
            currentAttackData.particle?.PlayVFX(_target.transform.position, Quaternion.identity);
            if(_target.TryCastDamage())
                _target.ApplyDamage(data, _entity);
            qteSuccesCnt = 0;
        }

        public override void EndAttack()
        {
            base.EndAttack();
        }
    }
}