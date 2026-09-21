using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SkeletonMageFallImpact
    {
        private readonly SkeletonMageFallingSlowFieldPatternSO _pattern;
        private readonly Collider2D[] _colliders = new Collider2D[16];
        public SkeletonMageFallImpact(SkeletonMageFallingSlowFieldPatternSO pattern) => _pattern = pattern;

        public void ApplyLandingImpact(Enemy owner, Vector2 landingPosition)
        {
            DamageTargets(owner, landingPosition);
            SpawnSlowField(landingPosition);
        }

        public void DamageTargets(Enemy owner, Vector2 landingPosition)
        {
            if (_pattern.ImpactRadius <= 0f || _pattern.Damage <= 0f)
                return;

            int count = Physics2D.OverlapCircle(landingPosition, _pattern.ImpactRadius, _pattern.WhatIsTarget, _colliders);
            DamageData damageData = new DamageData(_pattern.Damage, _pattern.DamageType);
            Vector2 direction = owner != null
                ? (landingPosition - (Vector2)owner.transform.position).normalized
                : Vector2.zero;

            for (int i = 0; i < count; i++)
            {
                Collider2D hitCollider = _colliders[i];
                if (hitCollider == null)
                    continue;

                IDamageable damageable = GetDamageable(hitCollider);
                damageable?.TakeDamage(damageData, direction, owner);
            }
        }

        public static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }

        public void SpawnSlowField(Vector2 position)
        {
            if (_pattern.SlowFieldRadius <= 0f || _pattern.SlowFieldDuration <= 0f)
                return;

            var fieldObject = new GameObject("SkeletonMageSlowField");
            var field = fieldObject.AddComponent<SlowFieldZone>();
            field.Initialize(
                position,
                _pattern.SlowFieldRadius,
                _pattern.SlowFieldDuration,
                _pattern.SlowSpeedMultiplier,
                _pattern.MoveSpeedStatName,
                _pattern.WhatIsTarget,
                _pattern.SlowFieldColor,
                _pattern.SlowFieldSegments,
                _pattern.SlowFieldSortingLayerName,
                _pattern.SlowFieldSortingOrder);
        }
    }
}
