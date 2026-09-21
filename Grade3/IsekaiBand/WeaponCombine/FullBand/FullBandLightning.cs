using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal sealed class FullBandLightning
    {
        private const int LightningCountPerBurst = 5;
        private const int LightningBurstCount = 3;
        private const float LightningBurstInterval = 0.3f;
        private const float LightningCooldown = 3f;
        private readonly FullBandAttack _source;
        private readonly FullBandStats _stats;
        private readonly FullBandAreaAttack _areaAttack;
        private readonly Collider2D[] _targetHits = new Collider2D[256];
        private readonly HashSet<int> _targetIds = new();
        private readonly List<Collider2D> _lightningTargets = new(LightningCountPerBurst);
        private float _lightningCooldownTimer;
        private float _lightningBurstTimer;
        private int _lightningBurstsRemaining;

        public FullBandLightning(FullBandAttack source, FullBandStats stats, FullBandAreaAttack areaAttack)
        {
            _source = source;
            _stats = stats;
            _areaAttack = areaAttack;
        }

        public void Reset()
        {
            _lightningCooldownTimer = 0f;
            _lightningBurstsRemaining = 0;
        }

        public void Tick(float deltaTime)
        {
            _lightningCooldownTimer -= deltaTime;
            _lightningBurstTimer -= deltaTime;

            if (_lightningBurstsRemaining <= 0 && _lightningCooldownTimer <= 0f)
            {
                _lightningBurstsRemaining = LightningBurstCount;
                _lightningBurstTimer = 0f;
                _lightningCooldownTimer = _source.ScaleCommonInterval(LightningCooldown);
            }

            if (_lightningBurstsRemaining <= 0 || _lightningBurstTimer > 0f)
                return;

            StrikeLightningVolley();
            _lightningBurstsRemaining--;
            _lightningBurstTimer = _source.ScaleCommonInterval(LightningBurstInterval);
        }

        private void StrikeLightningVolley()
        {
            CollectLightningTargets();
            if (_lightningTargets.Count == 0)
                return;

            for (int i = 0; i < LightningCountPerBurst; i++)
            {
                Collider2D target = _lightningTargets[i % _lightningTargets.Count];
                if (target == null) continue;

                _source.AttackAudio.Play(SoundKeys.ElectricProjectileDischarge);
                Vector3 strikePosition = target.bounds.center;
                strikePosition += (Vector3)(Random.insideUnitCircle * 0.1f);
                _areaAttack.DamageArea(strikePosition, _source.ScaleCommonRange(_stats.LightningRadius), _stats.LightningDamage, 0f);
                FullBandLightningVisual.Spawn(strikePosition, _source.ScaleCommonRange(_stats.LightningRadius), i);
            }
        }

        private void CollectLightningTargets()
        {
            _lightningTargets.Clear();
            _targetIds.Clear();

            int count = Physics2D.OverlapCircle(
                _source.Position,
                _source.ScaleCommonRange(_stats.LightningTargetRange),
                _source.TargetContactFilter,
                _targetHits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _targetHits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_targetIds.Add(targetId))
                    _lightningTargets.Add(hit);
            }

            Vector3 center = _source.Position;
            _lightningTargets.Sort((left, right) =>
            {
                float leftDistance = ((Vector2)(left.bounds.center - center)).sqrMagnitude;
                float rightDistance = ((Vector2)(right.bounds.center - center)).sqrMagnitude;
                return leftDistance.CompareTo(rightDistance);
            });

            if (_lightningTargets.Count > LightningCountPerBurst)
                _lightningTargets.RemoveRange(
                    LightningCountPerBurst,
                    _lightningTargets.Count - LightningCountPerBurst);
        }
    }
}
