using System.Threading;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossHumanSpiralPattern",
        menuName = "SO/Pattern/PianoBoss Human/Archimedean Spiral",
        order = 1)]
    public sealed class PianoBossHumanSpiralPatternSO : PianoBossHumanPatternBaseSO
    {
        [Header("Archimedean Spiral")]
        [SerializeField, Min(1)] private int projectileCount = 6;
        [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 0.5f);
        [SerializeField, Min(0f)] private float initialRadius = 0.6f;
        [SerializeField, Min(0f)] private float radialSpeed = 2.4f;
        [SerializeField, Min(0f)] private float angularSpeed = 90f;
        [SerializeField, Min(0f)] private float pathAcceleration = 0.65f;
        [SerializeField] private bool alternateRotationDirection = true;
        [SerializeField, Min(0.05f)] private float projectileLifeTime = 4.8f;
        [SerializeField, Min(0.05f)] private float projectileScale = 0.52f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.55f;
        [SerializeField, Min(0f)] private float patternLockDuration = 0.55f;

        [Header("Trail")]
        [SerializeField] private Color projectileColor = Color.white;
        [SerializeField] private Color trailColor = new Color(0.72f, 0.18f, 1f, 0.75f);
        [SerializeField, Min(0.05f)] private float trailTime = 0.42f;
        [SerializeField, Min(0.01f)] private float trailStartWidth = 0.24f;
        [SerializeField, Min(0f)] private float trailEndWidth = 0.02f;

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            int count = Mathf.Max(1, projectileCount);
            Vector2 center = (Vector2)owner.transform.position + spawnOffset;
            float startAngle = GetStartAngle(owner, center);
            float rotationSign = alternateRotationDirection && Random.value < 0.5f ? -1f : 1f;

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + 360f * i / count;
                SpawnSpiralProjectile(owner, center, angle, rotationSign, i);
            }

            if (patternLockDuration > 0f)
                await UniTask.WaitForSeconds(patternLockDuration, cancellationToken: ct);
        }

        private float GetStartAngle(Enemy owner, Vector2 center)
        {
            if (owner.target == null)
                return Random.Range(0f, 360f);

            Vector2 direction = (Vector2)owner.target.transform.position - center;
            return direction.sqrMagnitude > 0.001f
                ? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
                : Random.Range(0f, 360f);
        }

        private void SpawnSpiralProjectile(
            Enemy owner,
            Vector2 center,
            float initialAngle,
            float rotationSign,
            int spriteIndex)
        {
            Vector2 radialDirection = new Vector2(
                Mathf.Cos(initialAngle * Mathf.Deg2Rad),
                Mathf.Sin(initialAngle * Mathf.Deg2Rad));
            Vector2 spawnPosition = center + radialDirection * initialRadius;

            PianoBossHumanSpiralProjectile projectile = PianoBossHumanSpiralProjectile.Create(
                "PianoHumanArchimedeanNote",
                spawnPosition,
                GetNoteSprite(spriteIndex),
                projectileColor,
                projectileScale,
                visualSortingOrder);
            if (projectile == null)
                return;

            projectile.EnableTrail(trailColor, trailTime, trailStartWidth, trailEndWidth);
            projectile.Initialize(
                center,
                initialAngle,
                initialRadius,
                radialSpeed,
                angularSpeed * rotationSign,
                pathAcceleration,
                CreateNoteDamage(owner, damageMultiplier),
                projectileLifeTime,
                owner,
                whatIsTarget);
        }
    }
}
