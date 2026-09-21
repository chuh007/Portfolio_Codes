using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanSpiralMotion
    {
        private readonly PianoBossHumanSpiralProjectile _projectile;
        private Vector2 _center;
        private float _initialAngle;
        private float _initialRadius;
        private float _radialSpeed;
        private float _angularSpeed;
        private float _pathAcceleration;
        private float _lifeTime;
        private float _elapsed;

        public HumanSpiralMotion(PianoBossHumanSpiralProjectile projectile) => _projectile = projectile;

        public void Initialize(
            Vector2 center,
            float initialAngleDegrees,
            float initialRadius,
            float radialSpeed,
            float angularSpeedDegrees,
            float pathAcceleration,
            DamageData damage,
            float lifeTime,
            Entity owner,
            LayerMask targetMask)
        {
            _projectile.enabled = true;
            _center = center;
            _initialAngle = initialAngleDegrees * Mathf.Deg2Rad;
            _initialRadius = Mathf.Max(0f, initialRadius);
            _radialSpeed = radialSpeed;
            _angularSpeed = angularSpeedDegrees * Mathf.Deg2Rad;
            _pathAcceleration = Mathf.Max(0f, pathAcceleration);
            _projectile.Damage = damage;
            _lifeTime = Mathf.Max(0.05f, lifeTime);
            _projectile.Owner = owner;
            _projectile.TargetMask = targetMask;
            _elapsed = 0f;

            SampleSpiral();
        }

        public void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _lifeTime)
            {
                ProjectilePool.Push(_projectile.gameObject);
                return;
            }

            SampleSpiral();
        }

        public void SampleSpiral()
        {
            float pathProgress = _elapsed + 0.5f * _pathAcceleration * _elapsed * _elapsed;
            float speedMultiplier = 1f + _pathAcceleration * _elapsed;
            float angle = _initialAngle + _angularSpeed * pathProgress;
            float radius = _initialRadius + _radialSpeed * pathProgress;
            Vector2 radial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 tangent = new Vector2(-radial.y, radial.x);

            _projectile.transform.position = _center + radial * radius;
            Vector2 velocity = radial * (_radialSpeed * speedMultiplier)
                               + tangent * (radius * _angularSpeed * speedMultiplier);
            if (velocity.sqrMagnitude <= 0.0001f)
                return;

            _projectile.Direction = velocity.normalized;
            _projectile.transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Atan2(_projectile.Direction.y, _projectile.Direction.x) * Mathf.Rad2Deg);
        }
    }
}
