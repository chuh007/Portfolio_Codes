using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal sealed class FullBandProjectiles
    {
        private const string PianoNotePrefabPath = "LCH/RuntimePrefabs/PianoNoteProjectile";
        private const string DrumStickPrefabPath = "LCH/RuntimePrefabs/FullBandDrumStick";
        private const int PianoDirectionCount = 16;
        private const int DrumDirectionCount = 12;
        private readonly FullBandAttack _source;
        private readonly FullBandStats _stats;
        private readonly BandRuntimeObjects _runtimeProjectiles = new();
        private int _pianoVolleyIndex;
        private int _drumVolleyIndex;

        public FullBandProjectiles(FullBandAttack source, FullBandStats stats)
        {
            _source = source;
            _stats = stats;
        }

        public void Dispose() => _runtimeProjectiles.ReturnToPool();

        public void Remove(GameObject projectile) => _runtimeProjectiles.Remove(projectile);

        public void FirePianoVolley(Vector3 origin)
        {
            float rotation = _pianoVolleyIndex++ * 7.5f;
            int pianoCount = _source.CountProjectiles(PianoDirectionCount);
            for (int i = 0; i < pianoCount; i++)
            {
                float angle = rotation + 360f * i / pianoCount;
                Quaternion projectileRotation = Quaternion.Euler(0f, 0f, angle);
                GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                    PianoNotePrefabPath, "PianoNoteProjectile", origin, projectileRotation,
                    i % 2 == 0
                        ? new Color(0.28f, 1f, 0.92f, 1f)
                        : new Color(1f, 0.42f, 0.88f, 1f),
                    1.25f, 62);
                if (obj == null)
                    continue;

                obj.name = "FullBandPianoNote";

                if (!obj.TryGetComponent<KeyboardProjectile>(out KeyboardProjectile projectile))
                {
                    ProjectilePool.Push(obj);
                    continue;
                }

                projectile.Init(new KeyboardProjectileSettings
                {
                    TargetLayerMask = _source.TargetLayerMask,
                    Speed = _stats.PianoSpeed,
                    MaxRange = _source.ScaleCommonRange(_stats.PianoRange),
                    Damage = _source.ScaleCommonDamage(_stats.PianoDamage),
                    NoteIndex = _pianoVolleyIndex + i,
                    IsPiercing = true,
                    PierceCountRemaining = 2,
                    EventSource = _source,
                    ReturnedToPool = OnPianoNoteReturned,
                });
                _runtimeProjectiles.Add(obj);
                _source.AttackAudio.Play(SoundKeys.PianoAttack);
            }

            BuildVisualEffect.SpawnCircle(
                origin, 0.7f, new Color(0.3f, 1f, 0.9f, 0.58f), 0.16f, 60, true);
        }

        public void FireDrumVolley()
        {
            float rotation = _drumVolleyIndex++ * 15f;
            int drumCount = _source.CountProjectiles(DrumDirectionCount);
            for (int i = 0; i < drumCount; i++)
            {
                float angle = rotation + 360f * i / drumCount;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                    DrumStickPrefabPath, "FullBandDrumStick", _source.Position,
                    Quaternion.Euler(0f, 0f, angle),
                    new Color(1f, 0.62f, 0.12f, 1f), 1.35f, 63);
                if (obj == null)
                    continue;

                (obj.GetComponent<FullBandDrumProjectile>() ?? obj.AddComponent<FullBandDrumProjectile>()).Init(
                    _source, direction, _stats.DrumSpeed, _source.ScaleCommonRange(_stats.DrumRange), 0.32f);
                _runtimeProjectiles.Add(obj);
                _source.AttackAudio.Play(SoundKeys.DrumProjectileThrow);
            }
        }

        private void OnPianoNoteReturned(KeyboardProjectile projectile)
        {
            if (projectile != null)
                Remove(projectile.gameObject);
        }

        public void RemoveReturnedProjectiles() => _runtimeProjectiles.RemoveDestroyed();
    }
}
