using System.Threading;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "PianoBossCurvedProjectilePattern", menuName = "SO/Pattern/PianoBoss/Curved Projectiles", order = 1)]
    public sealed class PianoBossCurvedProjectilePatternSO : PianoBossFloorPatternSO
    {
        [Header("Curved Projectiles")]
        [SerializeField] private Sprite phase2ProjectileSprite;
        [SerializeField, Min(0f)] private float curvedProjectileCooldown = 10f;
        [SerializeField, Range(0f, 1f)] private float curvedProjectileChance = 0.65f;
        [SerializeField] private PianoBossFloorPatternSO fallbackFloorPattern;
        [SerializeField, Min(1)] private int curvedProjectileCount = 14;
        [SerializeField, Min(0.02f)] private float curvedProjectileInterval = 0.13f;
        [SerializeField, Min(0.05f)] private float curvedProjectileDuration = 2.7f;
        [SerializeField, Min(0f)] private float curvedProjectileLifeTime = 3f;
        [SerializeField, Min(0f)] private float curvedProjectileDamageMultiplier = 0.36f;
        [SerializeField, Min(0f)] private float bezierCurveStrength = 5.5f;
        [SerializeField, Min(0f)] private float cameraEdgePadding = 2f;

        public override BasePatternSO ResolveSelectionPattern(Enemy owner)
        {
            if (CanSelectProjectile(owner))
                return this;

            return fallbackFloorPattern != null ? fallbackFloorPattern : this;
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            PianoBossRuntime runtime = GetRuntime(owner);
            using (runtime.BeginPerformance())
            {
                EnsurePianoFloor(owner, runtime, CreateDamage(owner, FloorDamageMultiplier));
                if (runtime.IsPatternOnCooldown(this))
                {
                    await RunPianoFloorBurst(owner, runtime, ct);
                    return;
                }

                runtime.StartCooldown(this, curvedProjectileCooldown);
                await RunCurvedProjectileSequence(owner, ct);
            }
        }

        private bool CanSelectProjectile(Enemy owner)
        {
            if (owner == null)
                return false;

            PianoBossRuntime runtime = GetRuntime(owner);
            return !runtime.IsPatternOnCooldown(this) && Random.value <= Mathf.Clamp01(curvedProjectileChance);
        }

        private async UniTask RunCurvedProjectileSequence(Enemy owner, CancellationToken ct)
        {
            int count = Mathf.Max(1, curvedProjectileCount);
            for (int i = 0; i < count; i++)
            {
                FireBezierProjectile(owner, i);
                await UniTask.WaitForSeconds(curvedProjectileInterval, cancellationToken: ct);
            }

            await UniTask.WaitForSeconds(curvedProjectileDuration * 0.65f, cancellationToken: ct);
        }

        private void FireBezierProjectile(Enemy owner, int index)
        {
            Vector3 bossPosition = owner.transform.position;
            float side = index % 2 == 0 ? 1f : -1f;
            Rect bounds = GetArenaBounds(owner);
            float outsideMargin = Mathf.Max(bezierCurveStrength, bounds.width * 0.3f);
            ResolveHorizontalTravelBounds(
                bounds,
                outsideMargin,
                out float leftOutsideX,
                out float rightOutsideX);
            float startX = side > 0f ? rightOutsideX : leftOutsideX;
            float endX = side > 0f ? leftOutsideX : rightOutsideX;
            float passY = Random.Range(bounds.yMin, bounds.yMax);
            float yJitter = bounds.height * 0.22f;
            float startY = Mathf.Clamp(passY + Random.Range(-yJitter, yJitter), bounds.yMin, bounds.yMax);
            float endY = Mathf.Clamp(passY + Random.Range(-yJitter, yJitter), bounds.yMin, bounds.yMax);
            Vector3 start = new Vector3(startX, startY, bossPosition.z);
            Vector3 end = new Vector3(endX, endY, bossPosition.z);
            Vector3 bossCenterPassPoint = new Vector3(bossPosition.x, passY, bossPosition.z);
            Vector3 controlMidpoint = bossCenterPassPoint * (4f / 3f) - (start + end) * (1f / 6f);
            float horizontalBend = (start.x - end.x) * 0.28f;
            float verticalBend = bounds.height * Random.Range(-0.18f, 0.18f);
            Vector3 bend = new Vector3(horizontalBend, verticalBend, 0f);
            Vector3 controlA = controlMidpoint + bend;
            Vector3 controlB = controlMidpoint - bend;

            PianoBossProjectile projectile = PianoBossProjectile.Create(
                "PianoBezierNote",
                start,
                ResolveSprite(phase2ProjectileSprite, GetNoteSprite(index)),
                new Color(0.72f, 0.92f, 1f, 0.96f),
                0.48f,
                visualSortingOrder + 5);
            if (projectile == null)
                return;

            projectile.EnableTrail(new Color(0.42f, 0.86f, 1f, 0.72f), 0.36f, 0.32f, 0.02f);
            projectile.InitBezier(
                start,
                controlA,
                controlB,
                end,
                CreateDamage(owner, curvedProjectileDamageMultiplier),
                curvedProjectileDuration,
                curvedProjectileLifeTime,
                owner,
                whatIsTarget);
        }

        private void ResolveHorizontalTravelBounds(
            Rect arenaBounds,
            float outsideMargin,
            out float leftOutsideX,
            out float rightOutsideX)
        {
            leftOutsideX = arenaBounds.xMin - outsideMargin;
            rightOutsideX = arenaBounds.xMax + outsideMargin;

            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.orthographic)
                return;

            float cameraHalfWidth = mainCamera.orthographicSize * Mathf.Max(0.01f, mainCamera.aspect);
            float cameraCenterX = mainCamera.transform.position.x;
            leftOutsideX = Mathf.Min(
                leftOutsideX,
                cameraCenterX - cameraHalfWidth - cameraEdgePadding);
            rightOutsideX = Mathf.Max(
                rightOutsideX,
                cameraCenterX + cameraHalfWidth + cameraEdgePadding);
        }
    }
}
