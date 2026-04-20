using System;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH._01Scripts.Entities;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace Work.CHUH._01Scripts.Combat
{
    public class EnemyArrow : Projectile, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }

        private Rigidbody2D _rbCompo;
        
        private Pool _myPool;

        private float _spawnTime;
        
        private void Awake()
        {
            _rbCompo = GetComponent<Rigidbody2D>();
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
        
        public void ResetItem()
        {
            
        }
        
        public override void InitAndFire(Vector2 dir, float damage, float speed, Entity owner, LayerMask whatIsTarget)
        {
            _damage = damage;
            _speed = speed;
            _owner = owner;
            _whatIsTarget = whatIsTarget;
            _rbCompo.linearVelocity = dir * speed;
            _lifeTime = 5f; // Test
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (_spawnTime + _lifeTime < Time.time)
            {
                _myPool.Push(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _whatIsTarget) == 0) return;
            if (other.TryGetComponent(out IDamageble damageable))
            {
                damageable.TakeDamage(_damage);
                _myPool.Push(this);
            }
        }
    }
}
