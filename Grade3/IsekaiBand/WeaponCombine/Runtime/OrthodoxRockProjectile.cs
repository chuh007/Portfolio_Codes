using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class OrthodoxRockProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[24];
        private readonly HashSet<int> _hitIds = new();
        private OrthodoxRockBandAttack _source;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _speed;
        private float _range;
        private float _damage;
        private float _hitRadius;

        public void Init(
            OrthodoxRockBandAttack source,
            Vector2 direction,
            float speed,
            float range,
            float damage,
            float hitRadius)
        {
            enabled = true;
            _hitIds.Clear();
            _source = source;
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
            _origin = transform.position;
            _speed = Mathf.Max(0.1f, speed);
            _range = Mathf.Max(0.5f, range);
            _damage = Mathf.Max(0f, damage);
            _hitRadius = Mathf.Max(0.1f, hitRadius);
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool();
                return;
            }

            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            transform.Rotate(0f, 0f, 240f * Time.deltaTime);

            int count = Physics2D.OverlapCircle(transform.position, _hitRadius, _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;
                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_hitIds.Add(targetId))
                    _source.HitRock(hit, transform.position, _damage, _hitRadius);
            }

            if (Vector3.Distance(_origin, transform.position) >= _range)
                ReturnToPool();
        }

        private void ReturnToPool()
        {
            OrthodoxRockBandAttack source = _source;
            _source = null;
            source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
