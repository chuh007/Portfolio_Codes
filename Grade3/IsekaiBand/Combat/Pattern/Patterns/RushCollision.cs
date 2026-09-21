using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class RushCollision
    {
        private const float ObstacleStopPadding = 0.08f;
        private readonly RushPatternSO _pattern;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[10];
        private readonly RaycastHit2D[] _obstacleHitBuffer = new RaycastHit2D[8];
        public RushCollision(RushPatternSO pattern) => _pattern = pattern;

        public bool TryGetObstacleStopPosition(
            Collider2D bodyCollider,
            Vector2 ownerPosition,
            Vector2 direction,
            float checkDistance,
            out Vector2 stopPosition)
        {
            stopPosition = ownerPosition;
            if (bodyCollider == null
                || direction.sqrMagnitude <= Mathf.Epsilon
                || _pattern.ObstacleLayerMask == 0)
            {
                return false;
            }

            ContactFilter2D obstacleFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _pattern.ObstacleLayerMask,
                useTriggers = false
            };
            int hitCount = bodyCollider.Cast(
                direction.normalized,
                obstacleFilter,
                _obstacleHitBuffer,
                Mathf.Max(0f, checkDistance));
            if (hitCount <= 0)
                return false;

            float nearestDistance = float.PositiveInfinity;
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _obstacleHitBuffer[i];
                if (hit.collider == null || hit.distance >= nearestDistance)
                    continue;
                nearestDistance = hit.distance;
            }

            if (float.IsPositiveInfinity(nearestDistance))
                return false;

            float travelDistance = Mathf.Max(0f, nearestDistance - ObstacleStopPadding);
            stopPosition = ownerPosition + direction.normalized * travelDistance;
            return true;
        }

        public void TryDamageTargets(Enemy owner, DamageData damage, Vector2 from, Vector2 to, Vector2 fallbackDir, HashSet<IDamageable> damagedTargets)
        {
            if (!_pattern.WhatIsTarget.useLayerMask) return;

            Vector2 delta = to - from;
            float distance = delta.magnitude;
            Vector2 castDir = distance > Mathf.Epsilon ? delta / distance : fallbackDir;
            if (castDir.sqrMagnitude <= Mathf.Epsilon) return;

            float hitRadius = Mathf.Max(0f, _pattern.WarningWidth * 0.5f);
            float castDistance = Mathf.Max(distance, 0.01f);
            int hitCount = Physics2D.CircleCast(from, hitRadius, castDir, _pattern.WhatIsTarget, _hitBuffer, castDistance);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _hitBuffer[i].collider;
                if (hitCollider == null) continue;

                IDamageable damageable = GetDamageable(hitCollider);
                if (damageable == null || damagedTargets.Contains(damageable)) continue;

                damageable.TakeDamage(damage, castDir, owner);
                damagedTargets.Add(damageable);
            }
        }

        public static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }
    }
}
