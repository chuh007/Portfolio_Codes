using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class SymphonicRockBounceProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[64];
        private SymphonicRockAttack _source;
        private Transform _owner;
        private Vector2 _direction;
        private float _speed;
        private float _maxOwnerDistance;
        private float _bounceRange;
        private Collider2D _lastHit;
        private Collider2D _previousBounceTarget;
        private Vector3 _lastHitPosition;

        public void Init(
            SymphonicRockAttack source,
            Transform owner,
            Vector2 direction,
            float speed,
            float maxOwnerDistance,
            float bounceRange)
        {
            enabled = true;
            _lastHit = null;
            _previousBounceTarget = null;
            _lastHitPosition = Vector3.zero;
            _source = source;
            _owner = owner;
            _direction = direction.sqrMagnitude > 0.001f
                ? direction.normalized
                : Vector2.right;
            _speed = speed;
            _maxOwnerDistance = maxOwnerDistance;
            _bounceRange = bounceRange;
        }

        private void Update()
        {
            if (_source == null
                || _owner == null
                || Vector2.Distance(transform.position, _owner.position) > _maxOwnerDistance)
            {
                ReturnToPool();
                return;
            }

            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);

            if (_lastHit != null
                && Vector2.Distance(transform.position, _lastHitPosition) > 0.8f)
            {
                _lastHit = null;
            }

            int count = Physics2D.OverlapCircle(
                transform.position,
                0.34f,
                _source.TargetContactFilter,
                _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == _lastHit || !SymphonicBounceTargeting.IsValidTarget(hit))
                    continue;

                _source?.HitEnemy(hit, transform.position);
                _lastHit = hit;
                _lastHitPosition = transform.position;
                RedirectToNextTarget(hit);
                _previousBounceTarget = hit;
                break;
            }
        }

        private void RedirectToNextTarget(Collider2D current)
        {
            Collider2D next = SymphonicBounceTargeting.FindNext(transform.position, _bounceRange,
                _source.TargetContactFilter, _hits, current, _previousBounceTarget);
            if (next != null)
                _direction = ((Vector2)next.transform.position - (Vector2)transform.position).normalized;
        }

        private void ReturnToPool()
        {
            SymphonicRockAttack source = _source;
            _source = null;
            source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
