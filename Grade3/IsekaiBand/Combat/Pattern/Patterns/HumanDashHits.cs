using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanDashHits
    {
        private readonly PianoBossHumanDashPatternSO _pattern;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[12];
        public HumanDashHits(PianoBossHumanDashPatternSO pattern) => _pattern = pattern;

        public void DamageBetween(
            Enemy owner,
            DamageData damage,
            Vector2 from,
            Vector2 to,
            HashSet<IDamageable> damagedTargets)
        {
            if (!_pattern.WhatIsTarget.useLayerMask)
                return;

            Vector2 delta = to - from;
            float distance = delta.magnitude;
            if (distance <= Mathf.Epsilon)
                return;

            Vector2 direction = delta / distance;
            int hitCount = Physics2D.CircleCast(
                from,
                _pattern.WarningWidth * 0.45f,
                direction,
                _pattern.WhatIsTarget,
                _hitBuffer,
                distance);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = _hitBuffer[i].collider;
                IDamageable damageable = hit != null
                    ? hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>()
                    : null;
                if (damageable == null || !damagedTargets.Add(damageable))
                    continue;

                damageable.TakeDamage(damage, direction, owner);
            }
        }

        public static void MoveOwner(Enemy owner, Rigidbody2D body, Vector2 position)
        {
            if (body != null)
                body.position = position;

            Vector3 current = owner.transform.position;
            owner.transform.position = new Vector3(position.x, position.y, current.z);
        }

        public static float EaseOutCubic(float value)
        {
            float inverse = 1f - Mathf.Clamp01(value);
            return 1f - inverse * inverse * inverse;
        }
    }
}
