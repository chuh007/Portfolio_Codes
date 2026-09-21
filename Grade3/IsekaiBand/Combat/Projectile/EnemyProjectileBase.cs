using System;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Projectile
{
    // 적 총알 기반
    public class EnemyProjectileBase : MonoBehaviour, IPoolable
    {
        protected DamageData _damage;
        protected float _lifeTime;
        protected Vector2 _dir;
        protected float _speed;
        protected Entity _onwer;
        protected LayerMask _whatIsTarget;

        private float _timer;
        private Renderer[] _renderers;
        private string[] _defaultSortingLayers;
        private int[] _defaultSortingOrders;
        
        private void Awake()
        {
            CaptureDefaultRenderSettings();
        }

        public virtual void InitAndFire(Vector2 dir, DamageData damage, float speed, float lifeTime, Entity owner, LayerMask whatIsTarget)
        {
            Vector2 fireDir = dir.sqrMagnitude > Mathf.Epsilon ? dir.normalized : Vector2.right;

            _damage = damage;
            _lifeTime = lifeTime;
            _dir = fireDir;
            _speed = speed;
            _onwer = owner;
            _whatIsTarget = whatIsTarget;
            ApplyOwnerRenderLayer(owner);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg);
            transform.position += (Vector3)fireDir * speed * Time.deltaTime;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            transform.position += (Vector3)_dir * (_speed * Time.deltaTime);
            if (_lifeTime <= _timer)
            {
                _myPool.Push(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _whatIsTarget) == 0) return;
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage);
                _myPool.Push(this);
            }
        }
        
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        
        private Pool _myPool;
        
        public void ResetItem()
        {
            _timer = 0;
        }
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        private void CaptureDefaultRenderSettings()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _defaultSortingLayers = new string[_renderers.Length];
            _defaultSortingOrders = new int[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer renderer = _renderers[i];
                _defaultSortingLayers[i] = renderer.sortingLayerName;
                _defaultSortingOrders[i] = renderer.sortingOrder;
            }
        }

        private void ApplyOwnerRenderLayer(Entity owner)
        {
            if (_renderers == null)
                CaptureDefaultRenderSettings();

            bool isBossProjectile = owner is Enemy { IsBoss: true };
            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer renderer = _renderers[i];
                if (renderer == null)
                    continue;

                if (isBossProjectile)
                {
                    BossProjectileRenderLayer.ApplyTo(renderer, _defaultSortingOrders[i]);
                    BossProjectileOutline.ApplyTo(renderer);
                    continue;
                }

                renderer.sortingLayerName = _defaultSortingLayers[i];
                renderer.sortingOrder = _defaultSortingOrders[i];
            }
        }
    }
}
