using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class HardRockProjectile : MonoBehaviour
    {
        private const float HitRadius = 1.44f;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[64];
        private readonly HashSet<int> _hitTargetIds = new();
        private HardRockBandAttack _source;
        private Vector2 _direction;
        private Vector3 _start;
        private float _speed;
        private float _range;

        public void Init(HardRockBandAttack source, Vector2 direction, float speed, float range)
        {
            enabled = true;
            _hitTargetIds.Clear();
            _source = source;
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
            _speed = speed;
            _range = range;
            _start = transform.position;
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool();
                return;
            }

            Vector2 movement = _direction * (_speed * Time.deltaTime);
            float distance = movement.magnitude;
            int count = Physics2D.CircleCast(
                transform.position, HitRadius, _direction,
                _source.TargetContactFilter, _hits, distance);
            transform.position += (Vector3)movement;
            transform.Rotate(0f, 0f, 420f * Time.deltaTime);

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i].collider;
                if (hit == null || !hit.CompareTag("Enemy")) continue;
                if (!ManualTargetingService.IsValid(hit.transform)) continue;
                if (!_hitTargetIds.Add(HardRockBandAttack.GetTargetId(hit))) continue;
                _source.HitRock(hit, _hits[i].point);
            }

            if (Vector3.Distance(_start, transform.position) < _range) return;
            _source.BreakRock(transform.position);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            HardRockBandAttack source = _source;
            _source = null;
            source?.ProjectileEnded(gameObject);
            ProjectilePool.Push(gameObject);
        }
    }
}
