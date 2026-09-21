using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class BalladSpiral
    {
        private const string NotePrefabPath = "LCH/RuntimePrefabs/PianoNoteProjectile";
        private const int SpiralArmCount = 12;
        private const int NotesPerArm = 4;
        private const int NotePierceCount = 3;
        private const float NoteSpawnDelay = 0.06f;
        private readonly BalladBandAttack _source;
        private readonly BalladStats _stats;
        private readonly BandRuntimeObjects _notes;

        public BalladSpiral(BalladBandAttack source, BalladStats stats, BandRuntimeObjects notes)
        {
            _source = source;
            _stats = stats;
            _notes = notes;
        }

        public void Fire()
        {
            float randomRotation = Random.Range(0f, 360f);
            int projectileCount = _source.CountProjectiles(SpiralArmCount * NotesPerArm);
            for (int arm = 0; arm < SpiralArmCount; arm++)
            {
                float armAngle = randomRotation + 360f * arm / SpiralArmCount;
                float randomStartRadius = Random.Range(0.15f, 0.85f);
                int armNoteCount = projectileCount / SpiralArmCount
                                   + (arm < projectileCount % SpiralArmCount ? 1 : 0);
                for (int noteIndex = 0; noteIndex < armNoteCount; noteIndex++)
                {
                    float delay = noteIndex * NoteSpawnDelay;
                    float startRadius = randomStartRadius + noteIndex * 0.22f;
                    Vector2 offset = Quaternion.Euler(0f, 0f, armAngle) * Vector2.right * startRadius;
                    GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                        NotePrefabPath,
                        "BalladSpiralNote",
                        _source.Position + (Vector3)offset,
                        Quaternion.Euler(0f, 0f, armAngle),
                        arm % 2 == 0
                            ? new Color(0.42f, 0.92f, 1f, 1f)
                            : new Color(0.78f, 0.52f, 1f, 1f),
                        1.1f);
                    if (obj == null)
                        continue;

                    BandPianoProjectileSetup.DisableBuiltIn(obj);
                    (obj.GetComponent<BalladSpiralProjectile>() ?? obj.AddComponent<BalladSpiralProjectile>()).Init(
                        _source,
                        _source.Position,
                        armAngle,
                        startRadius,
                        delay,
                        _stats.RadialSpeed,
                        _stats.AngularSpeed,
                        _source.ScaleCommonRange(_stats.SpiralRadius),
                        _stats.Damage,
                        _source.ScaleCommonRange(_stats.HitRadius),
                        NotePierceCount);
                    _notes.Add(obj);
                    _source.AttackAudio.Play(SoundKeys.PianoAttack);
                }
            }

            BandRuntimeVisuals.SpawnSignatureBurst(
                _source.Position,
                Mathf.Min(4.2f, _source.ScaleCommonRange(_stats.SpiralRadius) * 0.42f),
                new Color(0.38f, 0.9f, 1f, 0.55f),
                new Color(0.78f, 0.48f, 1f, 0.62f),
                SpiralArmCount,
                randomRotation,
                0.48f,
                53);
        }
    }
}
