using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.EmotionalRock
{
    internal sealed class EmotionalRockBarrage
    {
        private const string BoltPrefabPath = "LCH/RuntimePrefabs/ElectricBoltProjectile";
        private const string PianoPrefabPath = "LCH/RuntimePrefabs/PianoNoteProjectile";
        private const string BassPrefabPath = "LCH/RuntimePrefabs/BassRuntimeProjectile";
        private readonly EmotionalRockBandAttack _source;
        private readonly EmotionalRockStats _stats;
        private readonly EmotionalRockWaves _waves;
        private readonly BandRuntimeObjects _projectiles;
        private int _nextInstrument = -1;

        public EmotionalRockBarrage(EmotionalRockBandAttack source, EmotionalRockStats stats,
            EmotionalRockWaves waves, BandRuntimeObjects projectiles)
        {
            _source = source;
            _stats = stats;
            _waves = waves;
            _projectiles = projectiles;
        }

        public void InitializeInstrument()
        {
            if (_nextInstrument < 0) _nextInstrument = Random.Range(0, 4);
        }

        public void FireNextInstrument()
        {
            int instrument = _nextInstrument;
            _nextInstrument = (_nextInstrument + 1) % 4;

            Transform target = ManualTargetingService.FindPriorityOrNearest(
                _source.Position,
                _source.ScaleCommonRange(_stats.ProjectileRange));
            Vector2 direction = target != null
                ? ((Vector2)target.position - (Vector2)_source.Position).normalized
                : Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude < 0.001f)
                direction = Vector2.right;

            if (instrument < 0 || instrument >= 3)
            {
                _waves.FireVocalWave(direction);
                return;
            }

            int projectileCount = _source.CountProjectiles(1);
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = (i - (projectileCount - 1) * 0.5f) * 10f;
                Vector2 shotDirection = Quaternion.Euler(0f, 0f, angle) * direction;
                switch (instrument)
                {
                    case 0:
                        SpawnBandProjectile(
                            BoltPrefabPath, "EmotionalRockGuitarBolt", shotDirection, target,
                            _stats.GuitarDamage, _stats.ProjectileSpeed * 1.25f, 0.28f,
                            new Color(0.3f, 0.76f, 1f, 1f), 1.15f, false, true);
                        break;
                    case 1:
                        SpawnBandProjectile(
                            PianoPrefabPath, "EmotionalRockPianoNote", shotDirection, target,
                            _stats.KeyboardDamage, _stats.ProjectileSpeed, 0.32f,
                            new Color(0.76f, 0.42f, 1f, 1f), 1.05f, true, false);
                        break;
                    case 2:
                        SpawnBandProjectile(
                            BassPrefabPath, "EmotionalRockBassPulse", shotDirection, target,
                            _stats.BassDamage, _stats.ProjectileSpeed * 0.62f, 0.48f,
                            new Color(1f, 0.54f, 0.18f, 1f), 1.7f, false, true, true);
                        break;
                }
            }
        }

        private void SpawnBandProjectile(
            string prefabPath, string objectName, Vector2 direction, Transform target,
            float damage, float speed, float hitRadius, Color color, float scale,
            bool homing, bool infinitePierce, bool applySlow = false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                prefabPath, objectName, _source.Position, Quaternion.Euler(0f, 0f, angle),
                color, scale, 55);
            if (obj == null)
                return;

            BandPianoProjectileSetup.DisableBuiltIn(obj);
            (obj.GetComponent<EmotionalRockProjectile>() ?? obj.AddComponent<EmotionalRockProjectile>()).Init(
                _source, target, direction, speed, _source.ScaleCommonRange(_stats.ProjectileRange),
                damage, hitRadius, homing, infinitePierce, applySlow);
            _projectiles.Add(obj);
            if (prefabPath == PianoPrefabPath)
                _source.AttackAudio.Play(SoundKeys.PianoAttack);
        }
    }
}
