using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DnaHelixSpawn
    {
        private readonly PianoBossDnaHelixPatternSO _pattern;

        public DnaHelixSpawn(PianoBossDnaHelixPatternSO pattern) => _pattern = pattern;

        public void SpawnHelixPair(
            Enemy owner,
            float centerX,
            float startY,
            float endY,
            float amplitude,
            float initialPhase,
            DamageData damage,
            int pairIndex)
        {
            Sprite sprite = _pattern.SelectSprite(_pattern.HelixProjectileSprite, _pattern.NoteSprite(pairIndex));
            SpawnStrandProjectile(
                "PianoDnaHelixNoteA",
                owner,
                centerX,
                startY,
                endY,
                amplitude,
                initialPhase,
                damage,
                sprite,
                _pattern.StrandAColor);
            SpawnStrandProjectile(
                "PianoDnaHelixNoteB",
                owner,
                centerX,
                startY,
                endY,
                amplitude,
                initialPhase + Mathf.PI,
                damage,
                sprite,
                _pattern.StrandBColor);
        }

        public void SpawnStrandProjectile(
            string objectName,
            Enemy owner,
            float centerX,
            float startY,
            float endY,
            float amplitude,
            float initialPhase,
            DamageData damage,
            Sprite sprite,
            Color color)
        {
            var startPosition = new Vector2(
                centerX + Mathf.Sin(initialPhase) * amplitude,
                startY);
            PianoBossProjectile projectile = PianoBossProjectile.Create(
                objectName,
                startPosition,
                sprite,
                color,
                _pattern.ProjectileScale,
                _pattern.VisualOrder + 6);
            if (projectile == null)
                return;

            projectile.EnableTrail(
                new Color(color.r, color.g, color.b, color.a * 0.72f),
                _pattern.TrailTime,
                _pattern.TrailStartWidth,
                _pattern.TrailEndWidth);
            projectile.InitDescendingHelix(
                centerX,
                startY,
                endY,
                amplitude,
                _pattern.HelixTurns,
                initialPhase,
                damage,
                _pattern.TravelDuration,
                _pattern.ProjectileLifeTime,
                owner,
                _pattern.TargetMask);
        }
    }
}
