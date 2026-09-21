using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.AttackCompo
{
    public class JapokEnemyAttackCompo : EnemyAttackCompo
    {
        [Header("Explosion VFX")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO explosionVfx;
        [SerializeField, Min(0.01f)] private float explosionVfxScale = 4f;

        private readonly List<Collider2D> _hits = new List<Collider2D>();
        private ContactFilter2D _filter;

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _filter.useLayerMask = true;
            _filter.layerMask = whatIsTarget;
        }

        public override void Attack(Action onAttackEnd = null)
        {
            if (TryUseSinglePattern(onAttackEnd)) return;

            base.Attack(onAttackEnd);

            Vector3 explosionPosition = transform.position;
            int count = Physics2D.OverlapCircle(explosionPosition, _attackRange, _filter, _hits);
            for (int i = 0; i < count; i++)
            {
                if (_hits[i].TryGetComponent(out IDamageable damageable))
                {
                    Vector2 dir = (_hits[i].transform.position - explosionPosition).normalized;
                    damageable.TakeDamage(new DamageData(_damage), dir, _enemy);
                }
            }

            onAttackEnd?.Invoke();
        }

        public void PlayExplosionVfx()
        {
            if (poolManager == null || explosionVfx == null)
                return;

            var pooled = poolManager.Pop(explosionVfx);
            if (pooled is IPooledVFX vfx)
            {
                vfx.Play(transform.position, explosionVfxScale, Vector2.right);
                return;
            }

            Debug.LogWarning($"[{nameof(JapokEnemyAttackCompo)}] {explosionVfx.name} prefab needs {nameof(IPooledVFX)}.");
            if (pooled != null)
                poolManager.Push(pooled);
        }
    }
}
