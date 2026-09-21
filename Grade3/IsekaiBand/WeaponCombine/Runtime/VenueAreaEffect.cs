using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class VenueAreaEffect
    {
        private const float VenueVisualInterval = 0.4f;
        private readonly PlayerAttackBase _source;
        private readonly Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _venueHits = new Collider2D[192];
        private readonly HashSet<int> _venueTargetIds = new();

        public VenueAreaEffect(PlayerAttackBase source,
            Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _damageEnemy = damageEnemy;
        }

        public void Damage(Vector3 center, float radius, float damage, float slowMultiplier, float slowDuration)
        {
            _venueTargetIds.Clear();
            int count = Physics2D.OverlapCircle(
                center,
                radius,
                _source.TargetContactFilter,
                _venueHits);

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _venueHits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_venueTargetIds.Add(targetId))
                    continue;

                _damageEnemy(hit, center, damage, radius, 0f);
                var slowable = hit.GetComponent<ISlowable>() ?? hit.GetComponentInParent<ISlowable>();
                slowable?.ApplySlow(slowMultiplier, _source.ScaleCommonSlowDuration(slowDuration));
            }
        }

        public static void Draw(Vector3 center, float radius, Color primary, Color secondary)
        {
            BuildVisualEffect.SpawnCircle(
                center,
                radius,
                primary,
                VenueVisualInterval + 0.08f,
                44);
            BuildVisualEffect.SpawnCircle(
                center,
                radius * 0.92f,
                secondary,
                VenueVisualInterval + 0.08f,
                43);
        }
    }
}
