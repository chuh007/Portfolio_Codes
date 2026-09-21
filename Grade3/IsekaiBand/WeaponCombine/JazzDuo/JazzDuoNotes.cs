using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzDuoNotes
    {
        private const int AdditionalPierceCount = 2;
        private const float TrailZoneSpawnInterval = 0.375f;
        private const float TrailZoneSpawnIntervalJitter = 0.5f;
        private const float MinimumTrailZoneSpawnInterval = 0.1f;
        private const float NoteDamageMultiplier = 1.2f;
        private readonly JazzDuoAttack _source;
        private readonly JazzDuoStats _stats;
        private readonly JazzDuoZones _zones;
        private int _noteIndex;

        public JazzDuoNotes(JazzDuoAttack source, JazzDuoStats stats, JazzDuoZones zones)
        {
            _source = source;
            _stats = stats;
            _zones = zones;
        }

        public void Fire(Vector3 origin)
        {
            Transform target = ManualTargetingService.FindPriorityOrNearest(origin, _source.ScaleCommonRange(_stats.NoteRange));
            if (target == null) return;

            Vector2 direction = ((Vector2)target.position - (Vector2)origin).normalized;
            int noteCount = _source.CountProjectiles(1);
            for (int i = 0; i < noteCount; i++)
            {
                float angle = (i - (noteCount - 1) * 0.5f) * 10f;
                Vector2 shotDirection = Quaternion.Euler(0f, 0f, angle) * direction;
                FireNote(origin, shotDirection);
            }
        }

        private void FireNote(Vector3 origin, Vector2 direction)
        {
            bool spawnedImpactZone = false;
            SpawnNoteProjectile(origin, direction, new KeyboardProjectileSettings
            {
                TargetLayerMask = _source.TargetLayerMask,
                Speed = _stats.NoteSpeed,
                MaxRange = _source.ScaleCommonRange(_stats.NoteRange),
                Damage = _source.ScaleCommonDamage(_stats.NoteDamage) * NoteDamageMultiplier,
                NoteIndex = _noteIndex++,
                IsPiercing = true,
                PierceCountRemaining = AdditionalPierceCount,
                HomingNotes = true,
                HomingStrength = 0.25f,
                EventSource = _source,
                PeriodicPositionEffectInterval = TrailZoneSpawnInterval,
                PeriodicPositionEffectIntervalProvider = GetTrailZoneSpawnInterval,
                PeriodicPositionEffect = _zones.PlaceDamageZone,
                ImpactEffect = impact =>
                {
                    if (spawnedImpactZone) return;
                    spawnedImpactZone = true;
                    _zones.PlaceDamageZone(impact.Position);
                }
            });
        }

        private static float GetTrailZoneSpawnInterval()
            => Mathf.Max(MinimumTrailZoneSpawnInterval,
                TrailZoneSpawnInterval + Random.Range(-TrailZoneSpawnIntervalJitter, TrailZoneSpawnIntervalJitter));

        private void SpawnNoteProjectile(
            Vector3 origin,
            Vector2 direction,
            KeyboardProjectileSettings settings)
        {
            if (direction.sqrMagnitude < 0.001f) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = ProjectilePool.Pop(
                "PianoNoteProjectile",
                origin,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null) return;

            if (obj.TryGetComponent<KeyboardProjectile>(out var projectile))
            {
                projectile.Init(settings);
                _source.AttackAudio.Play(SoundKeys.PianoAttack);
                return;
            }

            ProjectilePool.Push(obj);
        }
    }
}
