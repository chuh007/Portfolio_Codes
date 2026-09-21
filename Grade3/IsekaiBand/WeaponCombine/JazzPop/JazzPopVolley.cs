using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzPopVolley
    {
        private const string ProjectilePrefabPath = "LCH/RuntimePrefabs/PianoNoteProjectile";
        private const int ProjectileCount = 12;
        private const float OrbitDuration = 4f;
        private const float MinimumOrbitRadius = 3f;
        private const float TimeToMinimumOrbitRadius = 1.5f;
        private const float TravelHitRadius = 0.65f;
        private readonly JazzPopBandAttack _source;
        private readonly JazzPopStats _stats;
        private readonly BandRuntimeObjects _projectiles;

        public JazzPopVolley(JazzPopBandAttack source, JazzPopStats stats, BandRuntimeObjects projectiles)
        {
            _source = source;
            _stats = stats;
            _projectiles = projectiles;
        }

        public void FireSpringVolley()
        {
            float startAngle = Random.Range(0f, 45f);
            int projectileCount = _source.CountProjectiles(ProjectileCount);
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + 360f * i / projectileCount;
                GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                    ProjectilePrefabPath,
                    "JazzPopSpringNote",
                    _source.Position,
                    Quaternion.Euler(0f, 0f, angle),
                    i % 2 == 0
                        ? new Color(1f, 0.34f, 0.7f, 1f)
                        : new Color(0.32f, 0.9f, 1f, 1f),
                    2.15f,
                    54);
                if (obj == null)
                    continue;

                BandPianoProjectileSetup.DisableBuiltIn(obj);
                (obj.GetComponent<JazzPopSpringProjectile>() ?? obj.AddComponent<JazzPopSpringProjectile>()).Init(
                    _source,
                    _source.OwnerTransform,
                    angle,
                    _source.ScaleCommonRange(_stats.OutwardDistance),
                    _source.ScaleCommonRange(_stats.OrbitRadius),
                    _stats.OutwardDuration,
                    _stats.OutwardDuration + OrbitDuration,
                    _source.ScaleCommonRange(MinimumOrbitRadius),
                    TimeToMinimumOrbitRadius,
                    TravelHitRadius);
                _projectiles.Add(obj);
                _source.AttackAudio.Play(SoundKeys.PianoAttack);
            }
        }
    }
}
