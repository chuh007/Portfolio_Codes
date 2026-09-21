using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzPopSpringProjectile : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly HashSet<int> _hitTargetIds = new();

        private JazzPopBandAttack _source;
        private Transform _owner;
        private Vector3 _spawnCenter;
        private float _startAngle;
        private float _outwardDistance;
        private float _orbitRadius;
        private float _outwardDuration;
        private float _lifetime;
        private float _minimumOrbitRadius;
        private float _timeToMinimumOrbitRadius;
        private float _travelHitRadius;
        private float _elapsed;

        public void Init(
            JazzPopBandAttack source,
            Transform owner,
            float startAngle,
            float outwardDistance,
            float orbitRadius,
            float outwardDuration,
            float lifetime,
            float minimumOrbitRadius,
            float timeToMinimumOrbitRadius,
            float travelHitRadius)
        {
            enabled = true;
            _elapsed = 0f;
            _hitTargetIds.Clear();
            _source = source;
            _owner = owner;
            _spawnCenter = transform.position;
            _startAngle = startAngle;
            _outwardDistance = Mathf.Max(0.5f, outwardDistance);
            _orbitRadius = Mathf.Max(0.5f, orbitRadius);
            _outwardDuration = Mathf.Max(0.05f, outwardDuration);
            _lifetime = Mathf.Max(_outwardDuration + 0.1f, lifetime);
            _minimumOrbitRadius = Mathf.Clamp(minimumOrbitRadius, 0.5f, _orbitRadius);
            _timeToMinimumOrbitRadius = Mathf.Max(0.1f, timeToMinimumOrbitRadius);
            _travelHitRadius = Mathf.Max(0.1f, travelHitRadius);
        }

        private void Update()
        {
            if (_source == null)
            {
                ProjectilePool.Push(gameObject);
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed < _outwardDuration)
            {
                float t = Mathf.Clamp01(_elapsed / _outwardDuration);
                float angle = _startAngle + 52f * t * t;
                float radius = _outwardDistance * (1f - Mathf.Pow(1f - t, 2f));
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                transform.position = _spawnCenter + (Vector3)(direction * radius);
                transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
            }
            else
            {
                float orbitTime = _elapsed - _outwardDuration;
                Vector3 center = _owner != null ? _owner.position : _spawnCenter;
                float angle = _startAngle + 52f + orbitTime * 155f;
                float radialCycle = Mathf.Cos(orbitTime * Mathf.PI / _timeToMinimumOrbitRadius);
                float radialProgress = (radialCycle + 1f) * 0.5f;
                float radius = Mathf.Lerp(_minimumOrbitRadius, _orbitRadius, radialProgress);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                transform.position = center + (Vector3)(direction * radius);
                transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
            }

            transform.Rotate(0f, 0f, 220f * Time.deltaTime);
            DamagePassingTargets();
            if (_elapsed < _lifetime)
                return;

            _source.Explode(this, transform.position);
            ProjectilePool.Push(gameObject);
        }

        private void DamagePassingTargets()
        {
            int count = Physics2D.OverlapCircle(
                transform.position,
                _travelHitRadius,
                _source.TargetContactFilter,
                _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_hitTargetIds.Add(targetId))
                    _source.DamagePassingTarget(hit, transform.position);
            }
        }
    }
}
