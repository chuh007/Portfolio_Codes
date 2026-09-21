using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class BalladBassZone
    {
        private const float BassZoneVisualInterval = 0.16f;
        private readonly BalladBandAttack _source;
        private readonly BalladStats _stats;
        private readonly Collider2D[] _bassZoneHits = new Collider2D[192];
        private readonly HashSet<int> _bassZoneTargetIds = new();
        private float _bassZoneTickTimer;
        private float _bassZoneVisualTimer;

        public BalladBassZone(BalladBandAttack source, BalladStats stats)
        {
            _source = source;
            _stats = stats;
        }

        public void Tick(float deltaTime)
        {
            _bassZoneTickTimer += deltaTime;
            _bassZoneVisualTimer += deltaTime;

            float interval = _source.ScaleCommonInterval(_stats.BassZoneTickInterval);
            while (_bassZoneTickTimer >= interval)
            {
                _bassZoneTickTimer -= interval;
                DamageExpandedBassZone();
            }

            while (_bassZoneVisualTimer >= BassZoneVisualInterval)
            {
                _bassZoneVisualTimer -= BassZoneVisualInterval;
                float pulse = 1f + Mathf.Sin(Time.time * 5.5f) * 0.035f;
                BuildVisualEffect.SpawnCircle(
                    _source.Position,
                    _source.ScaleCommonRange(_stats.BassZoneRadius) * pulse,
                    new Color(0.18f, 0.74f, 0.68f, 0.24f),
                    BassZoneVisualInterval + 0.04f,
                    3,
                    sortingLayerName: GroundEffectRenderLayer.SortingLayerName);
            }
        }

        private void DamageExpandedBassZone()
        {
            _bassZoneTargetIds.Clear();
            int count = Physics2D.OverlapCircle(
                _source.Position,
                _source.ScaleCommonRange(_stats.BassZoneRadius),
                _source.TargetContactFilter,
                _bassZoneHits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _bassZoneHits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_bassZoneTargetIds.Add(targetId))
                    _source.DamageNote(hit, _source.Position, _stats.BassZoneDamage, _source.ScaleCommonRange(_stats.BassZoneRadius));
            }
        }
    }
}
