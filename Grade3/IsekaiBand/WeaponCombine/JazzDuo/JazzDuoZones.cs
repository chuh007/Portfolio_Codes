using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzDuoZones
    {
        private const float TrailZoneLifetime = 2f;
        private const float TrailZoneDamageInterval = 0.4f;
        private readonly JazzDuoAttack _source;
        private readonly JazzDuoStats _stats;
        private readonly System.Action<Collider2D, Vector3, float, float> _damageEnemy;
        private readonly BandRuntimeObjects _zones = new();
        private readonly Collider2D[] _zoneHits = new Collider2D[96];
        private readonly HashSet<int> _zoneHitIds = new();
        private bool _isDisposed;

        public JazzDuoZones(JazzDuoAttack source, JazzDuoStats stats,
            System.Action<Collider2D, Vector3, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _damageEnemy = damageEnemy;
        }

        public void Init() => _isDisposed = false;

        public void Tick() => _zones.RemoveDestroyed(includeInactive: false);

        public void Stop() => _isDisposed = true;

        public void Clear()
        {
            _zones.DestroyAll();
            _zoneHitIds.Clear();
        }

        public void DamageZone(Vector3 position, float radius)
        {
            if (_isDisposed) return;

            int count = Physics2D.OverlapCircle(position, radius, _source.TargetContactFilter, _zoneHits);
            _zoneHitIds.Clear();
            bool damagedAnyEnemy = false;
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _zoneHits[i];
                if (hit == null || !hit.CompareTag("Enemy")) continue;
                if (!_zoneHitIds.Add(BandRuntimeVisuals.GetTargetId(hit))) continue;
                _damageEnemy(hit, position, _stats.ZoneTickDamage, radius);
                damagedAnyEnemy = true;
            }

            if (!damagedAnyEnemy) return;
            _source.RaiseImpact(new AttackEventContext(
                _source, null, position, Vector2.zero, _stats.ZoneTickDamage, radius, radius,
                0f, 0f, 0, 0f, 0f));
        }

        public void PlaceDamageZone(Vector3 position)
        {
            if (_isDisposed) return;

            var obj = new GameObject("JazzDamageZone");
            obj.transform.position = position;
            var zone = obj.AddComponent<JazzDamageZone>();
            zone.Init(
                _source,
                _source.ScaleCommonRange(_stats.ZoneRadius),
                TrailZoneLifetime,
                TrailZoneDamageInterval);
            _zones.Add(obj);
        }
    }
}
