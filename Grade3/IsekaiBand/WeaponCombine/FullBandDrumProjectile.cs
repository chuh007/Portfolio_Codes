using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    /// <summary>
    /// 완전체 밴드의 드럼 스틱을 곡사 없이 직선으로 이동시킨다.
    /// 한 투사체가 같은 적에게 중복 충돌하지 않으며, 각 충돌은 공격 본체의 충격파를 호출한다.
    /// </summary>
    public sealed class FullBandDrumProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly HashSet<int> _hitTargetIds = new();

        private FullBandAttack _source;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _speed;
        private float _range;
        private float _hitRadius;

        public void Init(
            FullBandAttack source,
            Vector2 direction,
            float speed,
            float range,
            float hitRadius)
        {
            enabled = true;
            _source = source;
            _direction = direction.sqrMagnitude > 0.001f
                ? direction.normalized
                : Vector2.right;
            _origin = transform.position;
            _speed = Mathf.Max(0.1f, speed);
            _range = Mathf.Max(0.5f, range);
            _hitRadius = Mathf.Max(0.1f, hitRadius);
            _hitTargetIds.Clear();
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool();
                return;
            }

            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            transform.Rotate(0f, 0f, 420f * Time.deltaTime);
            DetectCollisions();

            if (Vector3.Distance(_origin, transform.position) >= _range)
                ReturnToPool();
        }

        private void DetectCollisions()
        {
            int count = Physics2D.OverlapCircle(
                transform.position,
                _hitRadius,
                _source.TargetContactFilter,
                _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_hitTargetIds.Add(targetId))
                    continue;

                _source.TriggerDrumCollision(hit.ClosestPoint(transform.position));
            }
        }

        private void ReturnToPool()
        {
            FullBandAttack source = _source;
            _source = null;
            source?.OnDrumProjectileReturned(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
