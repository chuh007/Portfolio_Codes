using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class ClosingRingSpawn
    {
        private readonly PianoBossHumanClosingRingPatternSO _pattern;

        public ClosingRingSpawn(PianoBossHumanClosingRingPatternSO pattern) => _pattern = pattern;

        public void SpawnClosingRing(Enemy owner, Vector2 center, int openDirection)
        {
            int count = Mathf.Max(8, _pattern.NoteCount);
            float radius = Mathf.Max(0.5f, _pattern.RingRadius);
            float speed = Mathf.Max(0.05f, _pattern.ApproachSpeed);
            float despawnRadius = Mathf.Clamp(_pattern.CenterDespawnRadius, 0f, radius - 0.05f);
            float lifeTime = Mathf.Max(0.05f, (radius - despawnRadius) / speed);
            float openAngle = openDirection * 90f;
            float openHalfAngle = Mathf.Clamp(_pattern.OpenSectorAngle, 20f, 140f) * 0.5f;
            DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
            int spriteIndex = 0;

            for (int i = 0; i < count; i++)
            {
                float angle = 360f * i / count;
                if (Mathf.Abs(Mathf.DeltaAngle(openAngle, angle)) <= openHalfAngle)
                    continue;

                SpawnClosingNote(owner, center, radius, speed, lifeTime, angle, damage, spriteIndex++);
            }
        }

        public void SpawnClosingNote(
            Enemy owner,
            Vector2 center,
            float radius,
            float speed,
            float lifeTime,
            float angle,
            DamageData damage,
            int spriteIndex)
        {
            float angleRadians = angle * Mathf.Deg2Rad;
            Vector2 radialDirection = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            Vector2 spawnPosition = center + radialDirection * radius;

            PianoBossHumanSpiralProjectile projectile = PianoBossHumanSpiralProjectile.Create(
                "PianoHumanClosingRingNote",
                spawnPosition,
                _pattern.NoteSprite(spriteIndex),
                _pattern.ProjectileColor,
                _pattern.ProjectileScale,
                _pattern.VisualOrder);
            if (projectile == null)
                return;

            projectile.EnableTrail(_pattern.TrailColor, _pattern.TrailTime, _pattern.TrailStartWidth, _pattern.TrailEndWidth);
            projectile.Initialize(
                center,
                angle,
                radius,
                -speed,
                0f,
                0f,
                damage,
                lifeTime,
                owner,
                _pattern.TargetMask);
        }
    }
}
