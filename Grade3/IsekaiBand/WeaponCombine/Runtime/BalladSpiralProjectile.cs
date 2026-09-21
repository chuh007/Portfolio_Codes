using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class BalladSpiralProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[24];
        private readonly HashSet<int> _hitIds = new();

        private BalladBandAttack _source;
        private Vector3 _center;
        private float _startAngle;
        private float _startRadius;
        private float _delay;
        private float _radialSpeed;
        private float _angularSpeed;
        private float _maxRadius;
        private float _damage;
        private float _hitRadius;
        private int _pierceCountRemaining;
        private float _elapsed;
        private SpriteRenderer[] _renderers;

        public void Init(
            BalladBandAttack source,
            Vector3 center,
            float startAngle,
            float startRadius,
            float delay,
            float radialSpeed,
            float angularSpeed,
            float maxRadius,
            float damage,
            float hitRadius,
            int pierceCount)
        {
            enabled = true;
            _elapsed = 0f;
            _hitIds.Clear();
            _source = source;
            _center = center;
            _startAngle = startAngle;
            _startRadius = startRadius;
            _delay = delay;
            _radialSpeed = Mathf.Max(0.1f, radialSpeed);
            _angularSpeed = angularSpeed;
            _maxRadius = Mathf.Max(startRadius + 0.1f, maxRadius);
            _damage = Mathf.Max(0f, damage);
            _hitRadius = Mathf.Max(0.1f, hitRadius);
            _pierceCountRemaining = Mathf.Max(0, pierceCount);
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            SetVisible(delay <= 0f);
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool();
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed < _delay)
                return;

            SetVisible(true);

            float age = _elapsed - _delay;
            float radius = _startRadius + _radialSpeed * age;
            float angle = _startAngle + _angularSpeed * age;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            transform.position = _center + (Vector3)(direction * radius);
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

            int count = Physics2D.OverlapCircle(transform.position, _hitRadius, _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;
                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_hitIds.Add(targetId))
                    continue;

                _source.DamageNote(hit, transform.position, _damage, _hitRadius);
                if (_pierceCountRemaining > 0)
                {
                    _pierceCountRemaining--;
                    continue;
                }

                ReturnToPool();
                return;
            }

            if (radius >= _maxRadius)
                ReturnToPool();
        }

        private void SetVisible(bool visible)
        {
            if (_renderers == null)
                return;
            foreach (SpriteRenderer renderer in _renderers)
                renderer.enabled = visible;
        }

        private void ReturnToPool()
        {
            BalladBandAttack source = _source;
            _source = null;
            source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
