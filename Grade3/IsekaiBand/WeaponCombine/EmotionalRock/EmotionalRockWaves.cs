using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.EmotionalRock
{
    internal sealed class EmotionalRockWaves
    {
        private const float PushKnockback = 1.8f;
        private const float VocalWaveDuration = 0.28f;
        private readonly EmotionalRockBandAttack _source;
        private readonly EmotionalRockStats _stats;
        private readonly Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _hits = new Collider2D[128];
        private readonly HashSet<int> _hitIds = new();

        public EmotionalRockWaves(EmotionalRockBandAttack source, EmotionalRockStats stats,
            Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _damageEnemy = damageEnemy;
        }

        public void FireVocalWave(Vector2 direction)
        {
            _source.AttackAudio.PlayVocal(VocalWaveDuration);
            const float angle = 82f;
            _hitIds.Clear();
            int count = Physics2D.OverlapCircle(_source.Position, _source.ScaleCommonRange(_stats.VocalRange), _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                Vector2 toTarget = (Vector2)hit.transform.position - (Vector2)_source.Position;
                if (!_hitIds.Add(targetId) || Vector2.Angle(direction, toTarget) > angle * 0.5f)
                    continue;

                _damageEnemy(hit, _source.Position, _stats.VocalDamage, _source.ScaleCommonRange(_stats.VocalRange), 1.2f);
            }

            BuildVisualEffect.SpawnConeWave(
                _source.Position,
                direction,
                _source.ScaleCommonRange(_stats.VocalRange),
                angle,
                new Color(0.86f, 0.26f, 1f, 0.66f),
                VocalWaveDuration,
                56);
        }

        public void FirePushWave(float interval)
        {
            _hitIds.Clear();
            int count = Physics2D.OverlapCircle(_source.Position, _source.ScaleCommonRange(_stats.PushRadius), _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;
                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_hitIds.Add(targetId))
                    _damageEnemy(hit, _source.Position, _stats.PushDamage, _source.ScaleCommonRange(_stats.PushRadius), PushKnockback);
            }

            BuildVisualEffect.SpawnCircle(
                _source.Position,
                _source.ScaleCommonRange(_stats.PushRadius),
                new Color(0.72f, 0.22f, 1f, 0.46f),
                interval * 0.92f,
                50);
            BuildVisualEffect.SpawnCircle(
                _source.Position,
                _source.ScaleCommonRange(_stats.PushRadius) * 0.58f,
                new Color(1f, 0.56f, 0.16f, 0.36f),
                interval * 0.72f,
                51,
                true);
        }
    }
}
