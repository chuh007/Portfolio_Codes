using System.Collections;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    internal class VfxDamageQuery
    {
        private readonly Collider2D[] _hitBuffer = new Collider2D[16];
        private readonly HashSet<IDamageable> _damagedTargets = new();

        public int HitAndGetCount(Transform transform, Vector2 hitBoxOffset, ContactFilter2D whatIsTarget,
            DamageData damage, Vector2 direction, Entity dealer, Vector2 worldSize)
        {
            if (!whatIsTarget.useLayerMask)
                return 0;

            _damagedTargets.Clear();

            Vector2 center = transform.TransformPoint(hitBoxOffset);
            int hitCount = Physics2D.OverlapBox(center, worldSize, transform.eulerAngles.z, whatIsTarget, _hitBuffer);
            return ApplyHits(hitCount, damage, direction, dealer);
        }

        public int HitCircleAndGetCount(Transform transform, Vector2 hitBoxOffset, ContactFilter2D whatIsTarget,
            DamageData damage, Vector2 direction, Entity dealer, float radius)
        {
            if (!whatIsTarget.useLayerMask)
                return 0;

            _damagedTargets.Clear();

            Vector2 center = transform.TransformPoint(hitBoxOffset);
            int hitCount = Physics2D.OverlapCircle(center, Mathf.Max(0f, radius), whatIsTarget, _hitBuffer);
            return ApplyHits(hitCount, damage, direction, dealer);
        }

        private int ApplyHits(int hitCount, DamageData damage, Vector2 direction, Entity dealer)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _hitBuffer[i];
                if (hitCollider == null)
                    continue;

                IDamageable damageable = GetDamageable(hitCollider);
                if (damageable == null || _damagedTargets.Contains(damageable))
                    continue;

                damageable.TakeDamage(damage, direction, dealer);
                _damagedTargets.Add(damageable);
            }

            return _damagedTargets.Count;
        }

        private static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }
    }
}
