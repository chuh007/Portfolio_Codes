using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class EmotionalRockProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly HashSet<int> _hitIds = new();

        private EmotionalRockBandAttack _source;
        private Transform _target;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _speed;
        private float _range;
        private float _damage;
        private float _hitRadius;
        private bool _homing;
        private bool _infinitePierce;
        private bool _applySlow;

        public void Init(
            EmotionalRockBandAttack source,
            Transform target,
            Vector2 direction,
            float speed,
            float range,
            float damage,
            float hitRadius,
            bool homing,
            bool infinitePierce,
            bool applySlow)
        {
            enabled = true;
            _hitIds.Clear();
            _source = source;
            _target = target;
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
            _origin = transform.position;
            _speed = Mathf.Max(0.1f, speed);
            _range = Mathf.Max(0.5f, range);
            _damage = Mathf.Max(0f, damage);
            _hitRadius = Mathf.Max(0.1f, hitRadius);
            _homing = homing;
            _infinitePierce = infinitePierce;
            _applySlow = applySlow;
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool();
                return;
            }

            if (_homing && _target != null && ManualTargetingService.IsValid(_target))
            {
                Vector2 desired = ((Vector2)_target.position - (Vector2)transform.position).normalized;
                _direction = Vector2.Lerp(_direction, desired, 7f * Time.deltaTime).normalized;
            }

            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);

            int count = Physics2D.OverlapCircle(transform.position, _hitRadius, _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_hitIds.Add(targetId))
                    continue;

                _source.HitProjectile(
                    this,
                    hit,
                    transform.position,
                    _damage,
                    _hitRadius,
                    _applySlow);
                if (!_infinitePierce)
                {
                    ReturnToPool();
                    return;
                }
            }

            if (Vector3.Distance(_origin, transform.position) >= _range)
                ReturnToPool();
        }

        private void ReturnToPool()
        {
            EmotionalRockBandAttack source = _source;
            _source = null;
            source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
