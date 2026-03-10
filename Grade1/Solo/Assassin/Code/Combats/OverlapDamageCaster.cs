using System;
using Code.Entities;
using UnityEditor;
using UnityEngine;

namespace Code.Combats
{
    
    public class OverlapDamageCaster : DamageCaster
    {
        public enum OverlapCastType
        {
            Circle, Box
        }
        [SerializeField] protected OverlapCastType overlapCastType;
        [SerializeField] private Vector2 damageBoxSize;
        [SerializeField] private float damageRadius;

        private Collider2D[] _hitResults;

        public override void InitCaster(Entity owner)
        {
            base.InitCaster(owner);
            _hitResults = new Collider2D[maxHitCount];
        }

        public override bool CastDamage(float damage, Vector2 knockBack, bool isPowerAttack)
        {
            
            int cnt = overlapCastType switch
            {
                OverlapCastType.Circle => Physics2D.OverlapCircle(transform.position, damageRadius, contactFilter, _hitResults),
                OverlapCastType.Box => Physics2D.OverlapBox(transform.position, damageBoxSize, 0, contactFilter, _hitResults),
                _ => 0
            };
            
            for (int i = 0; i < cnt; i++)
            {
                //피격 방향 구하기 ^_^
                Vector2 direction = (_hitResults[i].transform.position - _owner.transform.position).normalized;
                
                knockBack.x *= Mathf.Sign(direction.x);
            
                if (_hitResults[i].TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(damage, direction, knockBack, isPowerAttack, _owner);
                }
            }
            
            return cnt > 0;
        }

        public override void ApplyCounter(float damage, Vector2 direction, Vector2 knockBackForce, bool isPowerAttack, Entity dealer)
        {
            //이 부분은 나중에 투사체 캐스터는 다르게 해야한다.
            if (_owner is ICounterable counterable)
            {
                counterable.ApplyCounter(damage, direction, knockBackForce, isPowerAttack, dealer);
            }
        }

        public override Collider2D GetCounterableTarget(Vector3 center, LayerMask whatIsCounterable)
        {
            return overlapCastType switch
            {
                OverlapCastType.Circle =>
                    Physics2D.OverlapCircle(center, damageRadius, whatIsCounterable),
                OverlapCastType.Box =>
                    Physics2D.OverlapBox(center, damageBoxSize, 0, whatIsCounterable),
                _ => null
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.7f, 0.7f, 0, 1f);
            switch(overlapCastType)
            {
                case OverlapCastType.Circle:
                    Gizmos.DrawWireSphere(transform.position, damageRadius);
                    break;
                case OverlapCastType.Box:
                    Gizmos.DrawWireCube(transform.position, damageBoxSize);
                    break;
            };
        }
#endif
    }
}