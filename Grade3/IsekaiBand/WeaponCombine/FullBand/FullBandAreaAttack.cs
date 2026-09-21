using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal sealed class FullBandAreaAttack
    {
        private readonly FullBandAttack _source;
        private readonly FullBandStats _stats;
        private readonly Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _areaHits = new Collider2D[256];
        private readonly HashSet<int> _areaHitIds = new();

        public FullBandAreaAttack(FullBandAttack source, FullBandStats stats,
            Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _damageEnemy = damageEnemy;
        }

        public void EmitVocalPulse(float elapsed)
        {
            _source.AttackAudio.PlayVocal();
            DamageArea(_source.Position, _source.ScaleCommonRange(_stats.VocalRange), _stats.VocalDamage, 0f);
            BandRuntimeVisuals.SpawnSignatureBurst(
                _source.Position, _source.ScaleCommonRange(_stats.VocalRange),
                new Color(1f, 0.28f, 0.86f, 0.38f),
                new Color(0.35f, 0.92f, 1f, 0.52f),
                20, elapsed * 70f, 0.24f, 57);
        }

        public void EmitRhythmPulse()
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            DamageArea(_source.Position, _source.VenueRange, _stats.RhythmDamage, _stats.RhythmKnockback);

            if (!DrumWaveAnimationEffect.Spawn(
                    _source.Position, _source.VenueRange,
                    new Color(0.2f, 1f, 0.48f, 0.72f),
                    0.62f, 55))
            {
                BuildVisualEffect.SpawnCircle(
                    _source.Position, _source.VenueRange,
                    new Color(0.16f, 1f, 0.52f, 0.5f),
                    0.58f, 55, true);
            }
        }

        public void TriggerDrumCollision(Vector3 position)
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            DamageArea(position, _source.ScaleCommonRange(_stats.DrumShockwaveRadius), _stats.DrumDamage, _stats.DrumKnockback);

            if (!DrumWaveAnimationEffect.Spawn(
                    position, _source.ScaleCommonRange(_stats.DrumShockwaveRadius),
                    new Color(1f, 0.7f, 0.1f, 0.86f),
                    0.32f, 61))
            {
                BuildVisualEffect.SpawnCircle(
                    position, _source.ScaleCommonRange(_stats.DrumShockwaveRadius),
                    new Color(1f, 0.52f, 0.08f, 0.72f),
                    0.3f, 61, true);
            }
        }

        public void DamageArea(Vector3 center, float radius, float damage, float knockback)
        {
            _areaHitIds.Clear();
            int count = Physics2D.OverlapCircle(center, radius, _source.TargetContactFilter, _areaHits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _areaHits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_areaHitIds.Add(targetId))
                    _damageEnemy(hit, center, damage, radius, knockback);
            }

            _source.RaiseImpact(new AttackEventContext(
                _source, null, center, Vector2.zero, damage, radius, radius,
                0f, 0f, 0, 0f, knockback));
        }
    }
}
