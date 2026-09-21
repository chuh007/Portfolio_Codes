using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class FusionJazzProjectile : MonoBehaviour
    {
        private FusionJazzBandAttack _source;
        private Transform _target;
        private Collider2D _targetCollider;
        private float _speed;
        private float _lifetime;

        public void Init(
            FusionJazzBandAttack source,
            Transform target,
            Collider2D targetCollider,
            float speed,
            float lifetime)
        {
            enabled = true;
            _source = source;
            _target = target;
            _targetCollider = targetCollider;
            _speed = Mathf.Max(0.1f, speed);
            _lifetime = Mathf.Max(0.1f, lifetime);
        }

        private void Update()
        {
            if (_source == null || _target == null || !ManualTargetingService.IsValid(_target))
            {
                ReturnToPool();
                return;
            }

            _lifetime -= Time.deltaTime;
            Vector2 toTarget = (Vector2)_target.position - (Vector2)transform.position;
            float step = _speed * Time.deltaTime;
            if (toTarget.sqrMagnitude <= step * step || toTarget.sqrMagnitude <= 0.12f)
            {
                _source.HitTarget(this, _targetCollider, transform.position);
                ReturnToPool();
                return;
            }

            Vector2 direction = toTarget.normalized;
            transform.position += (Vector3)(direction * step);
            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            if (_lifetime <= 0f)
                ReturnToPool();
        }

        private void ReturnToPool()
        {
            FusionJazzBandAttack source = _source;
            _source = null;
            source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
