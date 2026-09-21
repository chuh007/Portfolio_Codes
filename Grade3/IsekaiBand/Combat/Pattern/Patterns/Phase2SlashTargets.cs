using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashTargets
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;

        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[10];
        public Phase2SlashTargets(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public void TryDamageTargets(
            Enemy owner,
            DamageData damage,
            Vector2 from,
            Vector2 to,
            Vector2 fallbackDirection,
            float hitRadius,
            HashSet<IDamageable> damagedTargets)
        {
            if (!_pattern.WhatIsTarget.useLayerMask)
                return;

            Vector2 delta = to - from;
            float distance = delta.magnitude;
            Vector2 castDirection = distance > Mathf.Epsilon ? delta / distance : fallbackDirection;
            if (castDirection.sqrMagnitude <= Mathf.Epsilon)
                return;

            float castDistance = Mathf.Max(distance, 0.01f);
            int hitCount = Physics2D.CircleCast(from, Mathf.Max(0f, hitRadius), castDirection, _pattern.WhatIsTarget, _hitBuffer, castDistance);
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _hitBuffer[i].collider;
                if (hitCollider == null)
                    continue;

                IDamageable damageable = GetDamageable(hitCollider);
                if (damageable == null || damagedTargets.Contains(damageable))
                    continue;

                damageable.TakeDamage(damage, castDirection, owner);
                damagedTargets.Add(damageable);
            }
        }

        public static Vector2 GetTargetDirection(Enemy owner)
        {
            if (owner == null || owner.target == null)
                return Vector2.right;

            return NormalizeDirection(owner.target.transform.position - owner.transform.position);
        }

        public static Vector2 NormalizeDirection(Vector2 direction)
        {
            return direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
        }

        public static bool ShouldStop(Enemy owner, CancellationToken ct)
        {
            return owner == null || owner.IsDead || ct.IsCancellationRequested;
        }

        public static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }
    }
}
