using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DoubleDashTargets
    {
        private readonly DoubleDashPatternSO _pattern;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[10];
        public DoubleDashTargets(DoubleDashPatternSO pattern) => _pattern = pattern;

        public void TryDamageTargets(
            Enemy owner,
            DamageData damage,
            Vector2 from,
            Vector2 to,
            Vector2 fallbackDir,
            HashSet<IDamageable> damagedTargets)
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

        public static Vector2 GetTargetDirection(Enemy owner)
        {
            return (owner.target.transform.position - owner.transform.position).normalized;
        }

        public static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }
    }
}
