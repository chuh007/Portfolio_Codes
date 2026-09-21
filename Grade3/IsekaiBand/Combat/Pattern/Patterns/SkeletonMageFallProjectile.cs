using System.Threading;
using _Code.LCH._02.Scripts.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SkeletonMageFallProjectile
    {
        private readonly SkeletonMageFallingSlowFieldPatternSO _pattern;

        public SkeletonMageFallProjectile(SkeletonMageFallingSlowFieldPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayProjectileFallAsync(
            Vector2 landingPosition,
            float duration,
            CancellationToken ct)
        {
            GameObject projectile = CreateProjectileObject(landingPosition + Vector2.up * _pattern.FallHeight);
            try
            {
                float elapsed = 0f;
                Vector2 startPosition = landingPosition + Vector2.up * _pattern.FallHeight;

                while (elapsed < duration)
                {
                    float t = Mathf.Clamp01(elapsed / duration);
                    SampleProjectileFall(projectile, startPosition, landingPosition, t);

                    elapsed += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                SampleProjectileFall(projectile, startPosition, landingPosition, 1f);
            }
            finally
            {
                if (projectile != null)
                    ProjectilePool.Push(projectile);
            }
        }

        public GameObject CreateProjectileObject(Vector2 position)
        {
            if (_pattern.ProjectileSprite == null)
                return null;

            GameObject projectile = ProjectilePool.Pop(
                "SkeletonMageFallingProjectile",
                position,
                Quaternion.identity);
            if (projectile == null)
                return null;

            projectile.name = "SkeletonMageFallingProjectile";
            projectile.transform.localScale = Vector3.one * _pattern.ProjectileScale;

            SpriteRenderer renderer = ProjectilePool.GetOrAddComponent<SpriteRenderer>(projectile);
            if (renderer == null)
            {
                Debug.LogError("[SkeletonMageFallingSlowFieldPatternSO] 투사체 SpriteRenderer를 생성하지 못했습니다.", projectile);
                ProjectilePool.Push(projectile);
                return null;
            }

            renderer.sprite = _pattern.ProjectileSprite;
            renderer.color = _pattern.ProjectileColor;
            renderer.sortingOrder = _pattern.ProjectileSortingOrder;
            if (!string.IsNullOrWhiteSpace(_pattern.ProjectileSortingLayerName))
                renderer.sortingLayerName = _pattern.ProjectileSortingLayerName;

            return projectile;
        }

        public void SampleProjectileFall(GameObject projectile, Vector2 startPosition, Vector2 landingPosition, float t)
        {
            if (projectile == null)
                return;

            float eased = t * t;
            projectile.transform.position = Vector2.Lerp(startPosition, landingPosition, eased);
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, _pattern.SpinDegrees * t);
        }
    }
}
